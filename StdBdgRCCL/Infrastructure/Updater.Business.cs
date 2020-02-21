using Microsoft.Extensions.Logging;
using StdBdgRCCL.Models;
using StdBdgRCCL.Models.AzureDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StdBdgRCCL.Infrastructure
{
    public partial class Updater
    {
        private static DateTime today = DateTime.Today;
        private static Tuple<string, string> quarter = Helpers.StringExt.CalculateQuarter(today);
        private static string date = DateTime.Now.ToShortDateString();
        private static DateTime dateOnly = Convert.ToDateTime(date);

        #region IC /studentEnrollments
        public async Task RunEdfiOdsSync(ICStudentEnrollment icStudentEnrollment)
        {
            string icStdNumber = icStudentEnrollment.StudentNumber.ToString();
            string icPersonId = icStudentEnrollment.PersonId.ToString();
            HttpResponse<EdfiEnrollmentStudent> edfiEnrollmentStudentResponse = await _athen.GetEdFiEnrollmentStudentByStudentUniqueId(icStdNumber, null);
            if (!edfiEnrollmentStudentResponse.IsSuccessStatusCode)
            {
                LoggerLQ.LogQueue($"Failed getting EdFi Enrollment Student { icStudentEnrollment.StudentNumber }");
                _logger.LogInformation($"Failed getting EdFi Enrollment Student { icStudentEnrollment.StudentNumber }");
                return;
            }
            EdfiEnrollmentStudent edfiEnrollmentStudent = edfiEnrollmentStudentResponse.ResponseContent;
            var edfiStudUniqueId = edfiEnrollmentStudent.StudentUniqueId;
            var edfiStudentId = edfiEnrollmentStudent.Id;
            //-----------------------------------------------------------------------------------------

            //Get StudentSchoolAssociations for enrollment/schools schoolId
            IDictionary<string, string> stdAssociationsRequestProperties = new Dictionary<string, string>()
            {
                { "studentUniqueId", edfiStudUniqueId }
            };
            HttpResponse<List<StudentSchoolAssociation>> edfiSSAResponse = await _athen.GetStudentSchoolAssociationsByStudentUniqueId(edfiStudUniqueId, stdAssociationsRequestProperties);
            if (!edfiSSAResponse.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Failed getting V3StudentSchoolAssociation for student { edfiStudUniqueId }");
                LoggerLQ.LogQueue($"Failed getting V3StudentSchoolAssociation for student {edfiStudUniqueId}");
                return;
            }
            List<StudentSchoolAssociation> listOfEdfiStudentSchoolAssociations = edfiSSAResponse.ResponseContent;
            EdfiEnrollmentSchool activeEdfiEnrollmentSchool = new EdfiEnrollmentSchool();
            List<StudentSchoolAssociation> activeSSAList = new List<StudentSchoolAssociation>();
            List<StudentSchoolAssociation> ssaWithdrawals = new List<StudentSchoolAssociation>();
            StudentSchoolAssociation activeSSA = new StudentSchoolAssociation();
            StudentSchoolAssociation ssaWithdrawal = new StudentSchoolAssociation();
            foreach (var ssa in listOfEdfiStudentSchoolAssociations)
            {
                if (string.IsNullOrEmpty(ssa.ExitWithdrawDate)) { activeSSAList.Add(ssa); }
                else if (!string.IsNullOrEmpty(ssa.ExitWithdrawDate)) { ssaWithdrawals.Add(ssa); }
            }

            if (activeSSAList.Count > 0)
            {
                activeSSA = activeSSAList[0];
                if (activeSSAList.Count > 1)
                {
                    _logger.LogInformation($"Multiple active enrollments for student {edfiStudUniqueId} - choosing latest date");
                    List<DateTime> dList = new List<DateTime>();
                    foreach (var date in activeSSAList)
                    {
                        var enDateTime = DateTime.Parse(date.EntryDate);
                        dList.Add(enDateTime);
                    }
                    string dT = dList.Max().ToString("yyyy-MM-dd");
                    activeSSA = activeSSAList.Where(x => x.EntryDate == dT).FirstOrDefault();
                }
            }
            if (ssaWithdrawals.Count > 0)
            {
                ssaWithdrawal = ssaWithdrawals[0];
                if (ssaWithdrawals.Count > 1)
                {
                    _logger.LogInformation($"Multiple withdrawals for student {edfiStudUniqueId} - choosing latest date");
                    List<DateTime> dList = new List<DateTime>();
                    foreach (var date in ssaWithdrawals)
                    {
                        var wdDateTime = DateTime.Parse(date.ExitWithdrawDate);
                        dList.Add(wdDateTime);
                    }
                    string dT = dList.Max().ToString("yyyy-MM-dd");
                    ssaWithdrawal = ssaWithdrawals.Where(x => x.ExitWithdrawDate == dT).FirstOrDefault();
                }
            }
            // Set edfiSSA to active School Association or SSA with latest withdrawal date
            var edfiSSA = activeSSA;

            if (activeSSAList.Count == 0 || activeSSA == null)
            {
                _logger.LogInformation($"No active enrollments for studentUniqueId {edfiStudUniqueId}");
                activeEdfiEnrollmentSchool = null;
                if (ssaWithdrawal != null)
                {
                    edfiSSA = ssaWithdrawal;
                }
            }

            //ServType
            string servType = "";
            if (activeSSA != null)
            {
                if (edfiSSA.PrimarySchool == true) { servType = "P"; }
                else { servType = "S"; }
            }

            //Get grade level
            string gradeLevel = "";
            string edfiGradeLevel = "";
            if (!string.IsNullOrEmpty(edfiSSA.EntryGradeLevelDescriptor) || !string.IsNullOrWhiteSpace(edfiSSA.EntryGradeLevelDescriptor))
            {
                gradeLevel = edfiSSA.EntryGradeLevelDescriptor.Substring(37);
                edfiGradeLevel = Helpers.EdFiGradeConverter.ConvertGrade(gradeLevel);
            }
            if (edfiGradeLevel == "Grade Conversion Failed")
            {
                _logger.LogInformation($"Failed converting grade for EdFi Enrollment Student {edfiStudUniqueId}");
                LoggerLQ.LogQueue($"Failed converting grade for EdFi Enrollment Student {edfiStudUniqueId}");
                return;
            }

            //Get enrollment/schools > IDCodes for BadgeStudent SchoolCode
            string enrollmentSchoolIDCode = "";
            string calendarYr = "";
            if (activeEdfiEnrollmentSchool != null)
            {
                HttpResponse<EdfiEnrollmentSchool> edfiV3EnrollmentSchoolResponse = await _athen.GetEdFiEnrollmentSchoolById(edfiSSA.SchoolReference.SchoolId);
                if (!edfiV3EnrollmentSchoolResponse.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Failed getting EdFiEnrollmentSchool for StudentSchoolAssociation { edfiSSA.SchoolReference.SchoolId }");
                    LoggerLQ.LogQueue($"Failed getting EdFiEnrollmentSchool for StudentSchoolAssociation {edfiSSA.SchoolReference.SchoolId}");
                    return;
                }
                activeEdfiEnrollmentSchool = edfiV3EnrollmentSchoolResponse.ResponseContent;
                if (activeEdfiEnrollmentSchool.IdentificationCodes.Count > 0)
                {
                    if (activeEdfiEnrollmentSchool.IdentificationCodes.Exists(c => c.EducationOrganizationIdentificationSystemDescriptor.Equals("uri://ed-fi.org/EducationOrganizationIdentificationSystemDescriptor#LEA")))
                    {
                        enrollmentSchoolIDCode = activeEdfiEnrollmentSchool.IdentificationCodes.Find(c =>
                        c.EducationOrganizationIdentificationSystemDescriptor.Equals("uri://ed-fi.org/EducationOrganizationIdentificationSystemDescriptor#LEA")).IDCode;
                    }
                    else
                    {
                        enrollmentSchoolIDCode = null;
                        _logger.LogInformation($"No LEA ID Code for enrollment school <{activeEdfiEnrollmentSchool.NameOfInstitution}> - studentId {edfiStudUniqueId}");
                    }
                }
                else
                {
                    _logger.LogInformation($"No identification codes in EdFiEnrollmentSchool { edfiStudUniqueId }");
                    LoggerLQ.LogQueue($"No identification codes in EdFiEnrollmentSchool {edfiStudUniqueId}");
                    return;
                }
                //-----------------------------------------------------------------------------------------
                //-----------------------------------------------------------------------------------------
                //Calendar year
                //Check quarter for correct year
                HttpResponse<Models.Calendar> calendar = new HttpResponse<Models.Calendar>();
                calendar = await _athen.GetCalendarBySchoolId(edfiSSA.SchoolReference.SchoolId, null);
                if (!calendar.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Request failure at GetCalendarBySchoolId({edfiSSA.SchoolReference.SchoolId})");
                    LoggerLQ.LogQueue($"Request failure at GetCalendarBySchoolId({edfiSSA.SchoolReference.SchoolId})");
                }
                if (calendar.Content != null)
                {
                    if (quarter.Item1 == "Q1" || quarter.Item1 == "Q2")
                    {
                        DateTime nYr = DateTime.Now.AddYears(1);
                        calendarYr = calendar.ResponseContent.SchoolYearTypeReference?.SchoolYear.ToString().Substring(2) + "-" + nYr.Year.ToString().Substring(2) + activeEdfiEnrollmentSchool.NameOfInstitution;
                    }
                    else if (quarter.Item1 == "Q3" || quarter.Item1 == "Q4")
                    {
                        DateTime lYr = DateTime.Now.AddYears(-1);
                        calendarYr = lYr.Year.ToString().Substring(2) + "-" + calendar.ResponseContent.SchoolYearTypeReference?.SchoolYear.ToString().Substring(2) + activeEdfiEnrollmentSchool.NameOfInstitution;
                    }
                    else if (quarter.Item1 == "Outside of school year") { _logger.LogInformation($"Quarter invlaid - Outside of school year"); }
                }
                else
                {
                    _logger.LogInformation($"Null calendar.Content at GetCalendarBySchoolId({edfiSSA.SchoolReference.SchoolId})");
                    //LoggerLQ.LogQueue($"Null calendar.Content at GetCalendarBySchoolId({edfiSSA.SchoolReference.SchoolId})");
                }
            }

            //-----------------------------------------------------------------------------------------
            var badgeStudentResponse = await GetBadgeStudentAsync(edfiStudUniqueId);
            if (!badgeStudentResponse.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Failure in badge student request {edfiStudUniqueId}");
                //LoggerLQ.LogQueue("Failure in badge student request.", edfiStudUniqueId);
                return;
            }
            BadgeStudent badgeStudent = badgeStudentResponse.ResponseContent;

            //-----------------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------------
            //Use edfiSSA (active) to find Location by schoolId (Phase 2 Implementation Jan2020)
            string homeroom = "";
            string homeroomCode = "";
            Location locatn;
            bool cbtpExists = false;
            if (activeEdfiEnrollmentSchool != null)
            {
                HttpResponse<List<Location>> locations = await _athen.GetLocationsBySchoolId(edfiSSA.SchoolReference.SchoolId, null);
                if (!locations.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Failed getting Locations by schoolId for school {edfiSSA.SchoolReference.SchoolId}");
                    //LoggerLQ.LogQueue($"Failed getting Locations by schoolId for school {edfiSSA.SchoolReference.SchoolId}");
                }
                if (locations.ResponseContent != null)
                {
                    cbtpExists = locations.ResponseContent.Exists(x => x.ClassroomIdentificationCode == "CBTP");
                    locatn = locations.ResponseContent.Where(x => x.ClassroomIdentificationCode == "CBTP").FirstOrDefault();
                    if (cbtpExists && locatn != null)
                    {
                        homeroom = locatn.ClassroomIdentificationCode;
                    }
                    else
                    {
                        // Do nothing
                        //_logger.LogInformation($"No classroom of 'CBTP' for school {edfiSSA.SchoolReference.SchoolId}");
                    }
                }
                else
                {
                    _logger.LogInformation($"Null locations content for school {edfiSSA.SchoolReference.SchoolId}");
                    LoggerLQ.LogQueue($"Null locations content for school {edfiSSA.SchoolReference.SchoolId}");
                }
                //====================================================================================================
                //Check course 'Homeroom 9-12 SX' exists in studentSchedules
                List<StudentSchedule> studentSchedule = new List<StudentSchedule>();
                var studentScheduleResponse = await _athen.GetStudentScheduleById(edfiStudUniqueId);
                if (!studentScheduleResponse.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Failed retrieving student schedule request for student {edfiStudUniqueId}");
                    LoggerLQ.LogQueue($"Failed retrieving student schedule request for student {edfiStudUniqueId}");
                    return;
                }
                if (studentScheduleResponse.ResponseContent.Count == 0)
                {
                    _logger.LogInformation($"No student schedules in studentSchedules resource for student {edfiStudUniqueId}");
                    LoggerLQ.LogQueue($"No student schedules in studentSchedules resource for student {edfiStudUniqueId}");
                    //return;
                }
                if (studentScheduleResponse.ResponseContent != null)
                {
                    studentSchedule = studentScheduleResponse.ResponseContent;
                }
                else
                {
                    _logger.LogInformation($"Null studentScheduleResponse.Content for student {edfiStudUniqueId}");
                    LoggerLQ.LogQueue($"Null studentScheduleResponse.Content for student {edfiStudUniqueId}");
                }
                //-----------------------------------------------------------------------------------------
                if (studentSchedule != null)
                {
                    Boolean hr9thru12Exists = studentSchedule.Exists(x => x.Course == ("Homeroom 9-12 " + quarter.Item2));
                    if (!string.IsNullOrWhiteSpace(homeroom) && !string.IsNullOrEmpty(homeroom) && hr9thru12Exists)
                    {
                        homeroomCode = homeroom;
                    }
                    else
                    {
                        _logger.LogInformation($"Null homeroomSection for student {edfiStudUniqueId}");
                        LoggerLQ.LogQueue($"Null homeroomSection for student {edfiStudUniqueId}");
                    }
                }
                else
                {
                    _logger.LogInformation($"Null studentSchedule for student {edfiStudUniqueId}");
                    LoggerLQ.LogQueue($"Null studentSchedule for student {edfiStudUniqueId}");
                }
            }

            await BadgeCRUD(icStudentEnrollment, badgeStudent, edfiSSA, edfiStudUniqueId, activeEdfiEnrollmentSchool, enrollmentSchoolIDCode, edfiEnrollmentStudent, edfiGradeLevel, homeroomCode, servType, calendarYr);
        }
        #endregion

        #region IC enrollment-changes
        public async Task RunEdfiOdsSync(ICEnrollmentChange icEnrollmtChange)
        {
            string icPersonId = icEnrollmtChange.PersonId.ToString();
            HttpResponse<ICStudentEnrollment> icStdEnrResponse = new HttpResponse<ICStudentEnrollment>();
            ICStudentEnrollment icStudentEnrollmt = new ICStudentEnrollment();
            icStdEnrResponse = await _athen.GetICStudentEnrollmentByPersonId(icPersonId);
            if (!icStdEnrResponse.IsSuccess)
            {
                LoggerLQ.LogQueue($"Failed getting IC Student Enrollment for personID {icPersonId}");
                _logger.LogInformation($"Failed getting IC Student Enrollment for personID {icPersonId}");
                return;
            }
            if(icStdEnrResponse.ResponseContent != null)
            {
                icStudentEnrollmt = icStdEnrResponse.ResponseContent;
            }
            string studentNumber = icStudentEnrollmt.StudentNumber.ToString();
            //-----------------------------------------------------------------------------------------
            //Get edfi enrollment student by studentUniqueId
            HttpResponse<EdfiEnrollmentStudent> edfiEnrollmentStudentResponse = await _athen.GetEdFiEnrollmentStudentByStudentUniqueId(studentNumber, null);
            if (!edfiEnrollmentStudentResponse.IsSuccessStatusCode)
            {
                LoggerLQ.LogQueue($"Failed getting EdFi Enrollment Student { studentNumber }");
                _logger.LogInformation($"Failed getting EdFi Enrollment Student { studentNumber }");
                return;
            }
            EdfiEnrollmentStudent edfiEnrollmentStudent = edfiEnrollmentStudentResponse.ResponseContent;
            var edfiStudUniqueId = edfiEnrollmentStudent.StudentUniqueId;
            var edfiStudentId = edfiEnrollmentStudent.Id;
            //-----------------------------------------------------------------------------------------
            //Get StudentSchoolAssociations for enrollment/schools schoolId
            IDictionary<string, string> stdAssociationsRequestProperties = new Dictionary<string, string>()
            {
                { "studentUniqueId", edfiStudUniqueId }
            };
            HttpResponse<List<StudentSchoolAssociation>> edfiSSAResponse = await _athen.GetStudentSchoolAssociationsByStudentUniqueId(edfiStudUniqueId, stdAssociationsRequestProperties);
            if (!edfiSSAResponse.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Failed getting V3StudentSchoolAssociation for student { edfiStudUniqueId }");
                LoggerLQ.LogQueue($"Failed getting V3StudentSchoolAssociation for student {edfiStudUniqueId}");
                return;
            }
            List<StudentSchoolAssociation> listOfEdfiStudentSchoolAssociations = edfiSSAResponse.ResponseContent;
            EdfiEnrollmentSchool activeEdfiEnrollmentSchool = new EdfiEnrollmentSchool();
            List<StudentSchoolAssociation> activeSSAList = new List<StudentSchoolAssociation>();
            List<StudentSchoolAssociation> ssaWithdrawals = new List<StudentSchoolAssociation>();
            StudentSchoolAssociation activeSSA = new StudentSchoolAssociation();
            StudentSchoolAssociation ssaWithdrawal = new StudentSchoolAssociation();
            foreach (var ssa in listOfEdfiStudentSchoolAssociations)
            {
                if (string.IsNullOrEmpty(ssa.ExitWithdrawDate)) { activeSSAList.Add(ssa); }
                else if (!string.IsNullOrEmpty(ssa.ExitWithdrawDate)) { ssaWithdrawals.Add(ssa); }
            }

            if (activeSSAList.Count > 0)
            {
                activeSSA = activeSSAList[0];
                if (activeSSAList.Count > 1)
                {
                    _logger.LogInformation($"Multiple active enrollments for student {edfiStudUniqueId} - choosing latest date");
                    List<DateTime> dList = new List<DateTime>();
                    foreach (var date in activeSSAList)
                    {
                        var enDateTime = DateTime.Parse(date.EntryDate);
                        dList.Add(enDateTime);
                    }
                    string dT = dList.Max().ToString("yyyy-MM-dd");
                    activeSSA = activeSSAList.Where(x => x.EntryDate == dT).FirstOrDefault();
                }
            }
            if (ssaWithdrawals.Count > 0)
            {
                ssaWithdrawal = ssaWithdrawals[0];
                if (ssaWithdrawals.Count > 1)
                {
                    _logger.LogInformation($"Multiple withdrawals for student {edfiStudUniqueId} - choosing latest date");
                    List<DateTime> dList = new List<DateTime>();
                    foreach (var date in ssaWithdrawals)
                    {
                        var wdDateTime = DateTime.Parse(date.ExitWithdrawDate);
                        dList.Add(wdDateTime);
                    }
                    string dT = dList.Max().ToString("yyyy-MM-dd");
                    ssaWithdrawal = ssaWithdrawals.Where(x => x.ExitWithdrawDate == dT).FirstOrDefault();
                }
            }
            // Set edfiSSA to active School Association or SSA with latest withdrawal date
            var edfiSSA = activeSSA;

            if (activeSSAList.Count == 0 || activeSSA == null)
            {
                _logger.LogInformation($"No active enrollments for studentUniqueId {edfiStudUniqueId}");
                activeEdfiEnrollmentSchool = null;
                if (ssaWithdrawal != null)
                {
                    edfiSSA = ssaWithdrawal;
                }
            }

            //ServType
            string servType = "";
            if (activeSSA != null)
            {
                if (edfiSSA.PrimarySchool == true) { servType = "P"; }
                else { servType = "S"; }
            }

            //Get grade level
            string gradeLevel = "";
            string edfiGradeLevel = "";
            if (!string.IsNullOrEmpty(edfiSSA.EntryGradeLevelDescriptor) || !string.IsNullOrWhiteSpace(edfiSSA.EntryGradeLevelDescriptor))
            {
                gradeLevel = edfiSSA.EntryGradeLevelDescriptor.Substring(37);
                edfiGradeLevel = Helpers.EdFiGradeConverter.ConvertGrade(gradeLevel);
            }
            if (edfiGradeLevel == "Grade Conversion Failed")
            {
                _logger.LogInformation($"Failed converting grade for EdFi Enrollment Student {edfiStudUniqueId}");
                LoggerLQ.LogQueue($"Failed converting grade for EdFi Enrollment Student {edfiStudUniqueId}");
                return;
            }

            //Get enrollment/schools > IDCodes for BadgeStudent SchoolCode
            string enrollmentSchoolIDCode = "";
            string calendarYr = "";
            if (activeEdfiEnrollmentSchool != null)
            {
                HttpResponse<EdfiEnrollmentSchool> edfiV3EnrollmentSchoolResponse = await _athen.GetEdFiEnrollmentSchoolById(edfiSSA.SchoolReference.SchoolId);
                if (!edfiV3EnrollmentSchoolResponse.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Failed getting EdFiEnrollmentSchool for StudentSchoolAssociation { edfiSSA.SchoolReference.SchoolId }");
                    LoggerLQ.LogQueue($"Failed getting EdFiEnrollmentSchool for StudentSchoolAssociation {edfiSSA.SchoolReference.SchoolId}");
                    return;
                }
                activeEdfiEnrollmentSchool = edfiV3EnrollmentSchoolResponse.ResponseContent;
                if (activeEdfiEnrollmentSchool.IdentificationCodes.Count > 0)
                {
                    if (activeEdfiEnrollmentSchool.IdentificationCodes.Exists(c => c.EducationOrganizationIdentificationSystemDescriptor.Equals("uri://ed-fi.org/EducationOrganizationIdentificationSystemDescriptor#LEA")))
                    {
                        enrollmentSchoolIDCode = activeEdfiEnrollmentSchool.IdentificationCodes.Find(c =>
                        c.EducationOrganizationIdentificationSystemDescriptor.Equals("uri://ed-fi.org/EducationOrganizationIdentificationSystemDescriptor#LEA")).IDCode;
                    }
                    else
                    {
                        enrollmentSchoolIDCode = null;
                        _logger.LogInformation($"No LEA ID Code for enrollment school <{activeEdfiEnrollmentSchool.NameOfInstitution}> - studentId {edfiStudUniqueId}");
                    }
                }
                else
                {
                    _logger.LogInformation($"No identification codes in EdFiEnrollmentSchool { edfiStudUniqueId }");
                    LoggerLQ.LogQueue($"No identification codes in EdFiEnrollmentSchool {edfiStudUniqueId}");
                    return;
                }
                //-----------------------------------------------------------------------------------------
                //-----------------------------------------------------------------------------------------
                //Calendar year
                //Check quarter for correct year
                HttpResponse<Models.Calendar> calendar = new HttpResponse<Models.Calendar>();
                calendar = await _athen.GetCalendarBySchoolId(edfiSSA.SchoolReference.SchoolId, null);
                if (!calendar.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Request failure at GetCalendarBySchoolId({edfiSSA.SchoolReference.SchoolId})");
                    LoggerLQ.LogQueue($"Request failure at GetCalendarBySchoolId({edfiSSA.SchoolReference.SchoolId})");
                }
                if (calendar.Content != null)
                {
                    if (quarter.Item1 == "Q1" || quarter.Item1 == "Q2")
                    {
                        DateTime nYr = DateTime.Now.AddYears(1);
                        calendarYr = calendar.ResponseContent.SchoolYearTypeReference?.SchoolYear.ToString().Substring(2) + "-" + nYr.Year.ToString().Substring(2) + activeEdfiEnrollmentSchool.NameOfInstitution;
                    }
                    else if (quarter.Item1 == "Q3" || quarter.Item1 == "Q4")
                    {
                        DateTime lYr = DateTime.Now.AddYears(-1);
                        calendarYr = lYr.Year.ToString().Substring(2) + "-" + calendar.ResponseContent.SchoolYearTypeReference?.SchoolYear.ToString().Substring(2) + activeEdfiEnrollmentSchool.NameOfInstitution;
                    }
                    else if (quarter.Item1 == "Outside of school year") { _logger.LogInformation($"Quarter invlaid - Outside of school year"); }
                }
                else
                {
                    _logger.LogInformation($"Null calendar.Content at GetCalendarBySchoolId({edfiSSA.SchoolReference.SchoolId})");
                    //LoggerLQ.LogQueue($"Null calendar.Content at GetCalendarBySchoolId({edfiSSA.SchoolReference.SchoolId})");
                }
            }

            //-----------------------------------------------------------------------------------------
            var badgeStudentResponse = await GetBadgeStudentAsync(edfiStudUniqueId);
            if (!badgeStudentResponse.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Failure in badge student request {edfiStudUniqueId}");
                //LoggerLQ.LogQueue("Failure in badge student request.", edfiStudUniqueId);
                return;
            }
            BadgeStudent badgeStudent = badgeStudentResponse.ResponseContent;
            //-----------------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------------
            //Use edfiSSA (active) to find Location by schoolId (Phase 2 Implementation Jan2020)
            string homeroom = "";
            string homeroomCode = "";
            Location locatn;
            bool cbtpExists = false;
            if (activeEdfiEnrollmentSchool != null)
            {
                HttpResponse<List<Location>> locations = await _athen.GetLocationsBySchoolId(edfiSSA.SchoolReference.SchoolId, null);
                if (!locations.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Failed getting Locations by schoolId for school {edfiSSA.SchoolReference.SchoolId}");
                    //LoggerLQ.LogQueue($"Failed getting Locations by schoolId for school {edfiSSA.SchoolReference.SchoolId}");
                }
                if (locations.ResponseContent != null)
                {
                    cbtpExists = locations.ResponseContent.Exists(x => x.ClassroomIdentificationCode == "CBTP");
                    locatn = locations.ResponseContent.Where(x => x.ClassroomIdentificationCode == "CBTP").FirstOrDefault();
                    if (cbtpExists && locatn != null)
                    {
                        homeroom = locatn.ClassroomIdentificationCode;
                    }
                    else
                    {
                        // Do nothing
                        //_logger.LogInformation($"No classroom of 'CBTP' for school {edfiSSA.SchoolReference.SchoolId}");
                    }
                }
                else
                {
                    _logger.LogInformation($"Null locations content for school {edfiSSA.SchoolReference.SchoolId}");
                    LoggerLQ.LogQueue($"Null locations content for school {edfiSSA.SchoolReference.SchoolId}");
                }
                //====================================================================================================
                //Check course 'Homeroom 9-12 SX' exists in studentSchedules
                List<StudentSchedule> studentSchedule = new List<StudentSchedule>();
                var studentScheduleResponse = await _athen.GetStudentScheduleById(edfiStudUniqueId);
                if (!studentScheduleResponse.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Failed retrieving student schedule request for student {edfiStudUniqueId}");
                    LoggerLQ.LogQueue($"Failed retrieving student schedule request for student {edfiStudUniqueId}");
                    return;
                }
                if (studentScheduleResponse.ResponseContent.Count == 0)
                {
                    _logger.LogInformation($"No student schedules in studentSchedules resource for student {edfiStudUniqueId}");
                    LoggerLQ.LogQueue($"No student schedules in studentSchedules resource for student {edfiStudUniqueId}");
                    //return;
                }
                if (studentScheduleResponse.ResponseContent != null)
                {
                    studentSchedule = studentScheduleResponse.ResponseContent;
                }
                else
                {
                    _logger.LogInformation($"Null studentScheduleResponse.Content for student {edfiStudUniqueId}");
                    LoggerLQ.LogQueue($"Null studentScheduleResponse.Content for student {edfiStudUniqueId}");
                }
                //-----------------------------------------------------------------------------------------
                if (studentSchedule != null)
                {
                    Boolean hr9thru12Exists = studentSchedule.Exists(x => x.Course == ("Homeroom 9-12 " + quarter.Item2));
                    if (!string.IsNullOrWhiteSpace(homeroom) && !string.IsNullOrEmpty(homeroom) && hr9thru12Exists)
                    {
                        homeroomCode = homeroom;
                    }
                    else
                    {
                        _logger.LogInformation($"Null homeroomSection for student {edfiStudUniqueId}");
                        LoggerLQ.LogQueue($"Null homeroomSection for student {edfiStudUniqueId}");
                    }
                }
                else
                {
                    _logger.LogInformation($"Null studentSchedule for student {edfiStudUniqueId}");
                    LoggerLQ.LogQueue($"Null studentSchedule for student {edfiStudUniqueId}");
                }
            }

            await BadgeCRUD(icStudentEnrollmt, badgeStudent, edfiSSA, edfiStudUniqueId, activeEdfiEnrollmentSchool, enrollmentSchoolIDCode, edfiEnrollmentStudent, edfiGradeLevel, homeroomCode, servType, calendarYr);
        }
        #endregion

        public async Task<HttpResponse<BadgeStudent>> GetBadgeStudentAsync(string studentId)
        {
            var badgeStudentResponse = await _athen.GetBadgeSystemStudentById(studentId);
            if (!badgeStudentResponse.IsSuccessStatusCode)
            {
                return new HttpResponse<BadgeStudent> { IsSuccess = false };
            }
            if (badgeStudentResponse.ResponseContent.Count == 0)
            {
                return new HttpResponse<BadgeStudent> { IsSuccess = true, ResponseContent = null };
            }
            else
            {
                return new HttpResponse<BadgeStudent> { IsSuccess = true, ResponseContent = badgeStudentResponse.ResponseContent[0] };
            }
        }

        public async Task BadgeCRUD(ICStudentEnrollment icStudentEnrollment, BadgeStudent badgeStudent, StudentSchoolAssociation edfiSSA, string edfiStudUniqueId, EdfiEnrollmentSchool activeEdFiEnrollmentSchool,
                                    string enrollmentSchoolIDCode, EdfiEnrollmentStudent edfiV3EnrollmentStudent, string edfiGradeLevel, string homeroomCode, string servType, string calendar)
        {
            //Try update
            if (badgeStudent != null && activeEdFiEnrollmentSchool != null)
            {
                long badgeChecksum = Helpers.HashConverter.Hash(badgeStudent.LastName?.ToUpper() +
                                      badgeStudent.FirstName?.ToUpper() +
                                      badgeStudent.MiddleName?.ToUpper() +
                                      badgeStudent.BirthDate?.ToString() +
                                      badgeStudent.SchoolNumber +
                                      badgeStudent.SchoolName?.ToUpper() +
                                      badgeStudent.Grade +
                                      badgeStudent.Homeroom?.ToUpper() +
                                      badgeStudent.ServType?.ToUpper() +
                                      badgeStudent.Calendar?.ToUpper() +
                                      badgeStudent.PrevMifareCardNum +
                                      badgeStudent.PersonId);

                long edfiEnrollmentChecksum = Helpers.HashConverter.Hash(edfiV3EnrollmentStudent.LastSurname.ToUpper() +
                                                               edfiV3EnrollmentStudent.FirstName.ToUpper() +
                                                               edfiV3EnrollmentStudent.MiddleName.ToUpper() +
                                                               edfiV3EnrollmentStudent.BirthDate.ToString() +
                                                               enrollmentSchoolIDCode +
                                                               activeEdFiEnrollmentSchool.NameOfInstitution.ToUpper() +
                                                               edfiGradeLevel +
                                                               homeroomCode.ToUpper() +
                                                               servType.ToUpper() +
                                                               calendar.ToUpper() +
                                                               badgeStudent.MifareCardNumber +
                                                               icStudentEnrollment.PersonId);
                //Update record ==============================================================
                if (badgeChecksum != edfiEnrollmentChecksum)
                {
                    //-----------
                    string stdSchoolNum = badgeStudent.SchoolNumber;
                    string stdSchoolName = badgeStudent.SchoolName;
                    string prevMifareCard = badgeStudent.PrevMifareCardNum;
                    //-----------
                    badgeStudent.LastName = edfiV3EnrollmentStudent.LastSurname;
                    badgeStudent.FirstName = edfiV3EnrollmentStudent.FirstName;
                    badgeStudent.MiddleName = edfiV3EnrollmentStudent.MiddleName;
                    badgeStudent.BirthDate = edfiV3EnrollmentStudent.BirthDate;
                    if (edfiSSA.ExitWithdrawDate != null)
                    {
                        badgeStudent.Expiration = Convert.ToDateTime(edfiSSA.ExitWithdrawDate);
                        badgeStudent.Expiredate = Convert.ToDateTime(edfiSSA.ExitWithdrawDate);
                    }
                    badgeStudent.SchoolNumber = enrollmentSchoolIDCode ?? "";
                    badgeStudent.SchoolName = activeEdFiEnrollmentSchool.NameOfInstitution ?? "";
                    badgeStudent.Grade = edfiGradeLevel;
                    badgeStudent.Homeroom = homeroomCode;
                    if (badgeStudent.Homeroom == "CBTP")
                    {
                        _logger.LogInformation($"Setting homeroom to CBTP for student {badgeStudent.StudentId}");
                        LoggerLQ.LogQueue($"Setting homeroom to CBTP for student {badgeStudent.StudentId}");
                    }
                    badgeStudent.PersonId = icStudentEnrollment.PersonId.ToString();
                    badgeStudent.ServType = servType;
                    badgeStudent.Calendar = calendar;
                    badgeStudent.LastUpdated = dateOnly;
                    badgeStudent.LastUpdatedBy = "Ed-fi Dev";

                    //Set badge types
                    if (badgeStudent.Grade == "P3" || badgeStudent.Grade == "P4" || badgeStudent.Grade == "K") { badgeStudent.BadgeType = "MNPS_Student_Badge_Non_Bus_Pass"; }
                    else
                    {
                        byte grade = Convert.ToByte(badgeStudent.Grade);
                        if (Enumerable.Range(9, 12).Contains(grade)) { badgeStudent.BadgeType = "MNPS_Student_Badge"; }
                        else if (Enumerable.Range(1, 8).Contains(grade)) { badgeStudent.BadgeType = "MNPS_Student_Badge_Non_Bus_Pass"; }
                    }

                    //Set certain fields null when school is changed
                    string[] exceptSchools = new string[] { "830", "803", "748", "397", "564" };
                    if ((badgeStudent.SchoolNumber != stdSchoolNum || badgeStudent.SchoolName != stdSchoolName) && !exceptSchools.Any(x => x == badgeStudent.SchoolNumber))
                    {
                        badgeStudent.PrintDate = null;
                        badgeStudent.MifareCardNumber = null;
                        badgeStudent.MifareBusPassNum = null;
                        badgeStudent.LastUpdatedBy = "SchoolChange";
                        badgeStudent.LastUpdated = dateOnly;
                    }

                    if (badgeStudent.SchoolNumber == "830" || icStudentEnrollment.CalendarName.Contains("Genesis") || icStudentEnrollment.CalendarName.Contains("High Road"))
                    {
                        badgeStudent.SchoolName = activeEdFiEnrollmentSchool.NameOfInstitution;
                        badgeStudent.BadgeType = "MNPS_SPED_BADGE";
                    }

                    if (prevMifareCard != badgeStudent.MifareCardNumber)
                    {
                        badgeStudent.IssueDate = dateOnly;
                    }

                    if (string.IsNullOrEmpty(badgeStudent.MifareCardNumber) && badgeStudent.Expiration == null)
                    {
                        badgeStudent.IssueDate = dateOnly;
                    }

                    // V6 Docs - disregard for now
                    //if (badgeStudent.MifareCardNumber == null)
                    //{
                    //    //Send record to MTA deactivation list
                    //}

                    if ((badgeStudent.SchoolName == "Harris-Hillman Special Education" || edfiSSA.SchoolReference.SchoolId == 1900302) && badgeStudent.Homeroom == "CBTP")
                    {
                        //badgeStudent.SchoolName = "CBTP";
                        badgeStudent.BadgeType = "MNPS_SPED_BADGE";
                    }

                    //========================================================================

                    _logger.LogInformation($"Updating {edfiStudUniqueId}");
                    var repoResponse = await _athen.UpdateStudentBadge(badgeStudent.StudentId, badgeStudent);
                    Thread.Sleep(200);
                    bool updateSuccess = false;
                    if (repoResponse.HttpRespMsg.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"Successfully updated student badge {badgeStudent.StudentId}");
                        LoggerLQ.LogQueue($"Successfully updated student badge {badgeStudent.StudentId}");
                        updateSuccess = true;
                    }
                    else
                    {
                        _logger.LogInformation($"Failed updating student badge {badgeStudent.StudentId}");
                        LoggerLQ.LogQueue($"Failed updating student badge {badgeStudent.StudentId}");
                    }

                    //if (updateSuccess)
                    //{
                    //    _logger.LogInformation($"Posting new student-historicals record for {badgeStudent.StudentId}");
                    //    Thread.Sleep(200);
                    //    PersonsHistory pHistoryObject = new PersonsHistory();

                    //    pHistoryObject.Bmsid = badgeStudent.StudentId;
                    //    pHistoryObject.LastName = badgeStudent.LastName;
                    //    pHistoryObject.FirstName = badgeStudent.FirstName;
                    //    pHistoryObject.MiddleName = badgeStudent.MiddleName;
                    //    pHistoryObject.SchoolName = badgeStudent.SchoolName;
                    //    pHistoryObject.SchoolNumber = badgeStudent.SchoolNumber;
                    //    pHistoryObject.Grade = badgeStudent.Grade;
                    //    pHistoryObject.MifareCardNumber = badgeStudent.MifareCardNumber;
                    //    pHistoryObject.MifareBusPassNum = badgeStudent.MifareBusPassNum;
                    //    pHistoryObject.MifareApproved = badgeStudent.MifareApproved;
                    //    pHistoryObject.MifareEligible = badgeStudent.MifareEligible;
                    //    var x = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff");
                    //    pHistoryObject.TimeStamp = Convert.ToDateTime(x);
                    //    pHistoryObject.HashkeyExist = badgeStudent.Hashkey;
                    //    pHistoryObject.LastUpdatedBy = badgeStudent.LastUpdatedBy;
                    //    pHistoryObject.PrintDate = badgeStudent.PrintDate;
                    //    pHistoryObject.IssueDate = badgeStudent.IssueDate != null ? badgeStudent.IssueDate.Value.ToString("yyyy-MM-dd") : DateTime.UtcNow.ToString("yyyy-MM-dd");
                    //    pHistoryObject.BadgeType = badgeStudent.BadgeType;

                    //    var resp = await _repo.PostPersonsHistoryToStudentHistoricals(pHistoryObject);
                    //    if (resp.Response.IsSuccessStatusCode)
                    //    {
                    //        LoggerLQ.LogQueue($"Successfully posted student-historicals object {pHistoryObject.Bmsid} for badge student update");
                    //        _logger.LogInformation($"Successfully posted student-historicals object {pHistoryObject.Bmsid} for normal badge student update");
                    //    }
                    //    else
                    //    {
                    //        LoggerLQ.LogQueue($"Failed to post student-historicals object {pHistoryObject.Bmsid} for badge student update");
                    //        _logger.LogInformation($"Failed to post student-historicals object {pHistoryObject.Bmsid} - normal badge student update. \r\n");
                    //    }
                    //}
                    return;
                }
            }
            //Delete record
            //No longer delete - Nov 2019 - update expiredate/expiration for badge student
            else if (badgeStudent != null && activeEdFiEnrollmentSchool == null)
            {
                DateTime? withdrawalExpiration = badgeStudent.Expiration;
                if (edfiSSA.ExitWithdrawDate != null)
                {
                    badgeStudent.Expiration = Convert.ToDateTime(edfiSSA.ExitWithdrawDate);
                    badgeStudent.Expiredate = Convert.ToDateTime(edfiSSA.ExitWithdrawDate);
                }

                if (withdrawalExpiration != badgeStudent.Expiration)
                {
                    badgeStudent.LastUpdated = dateOnly;
                    badgeStudent.LastUpdatedBy = "ed-fi dev";
                    _logger.LogInformation($"Updating {edfiStudUniqueId}");
                    Thread.Sleep(500);
                    var repoResponse = await _athen.UpdateStudentBadge(badgeStudent.StudentId, badgeStudent);
                    bool updateSuccess = false;
                    if (repoResponse.HttpRespMsg.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"Successfully updated {badgeStudent.StudentId} for badge student withdrawal.");
                        updateSuccess = true;
                    }
                    else
                    {
                        _logger.LogInformation($"Failed updating withdrawal for student {edfiStudUniqueId}");
                        LoggerLQ.LogQueue($"Failed updating withdrawal for student {edfiStudUniqueId}");
                    }
                    //if (updateSuccess)
                    //{
                    //    //POST to PersonsHistory table
                    //    var x = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff");
                    //    Thread.Sleep(500);
                    //    _logger.LogInformation($"Posting new student-historicals record {badgeStudent.StudentId} for withdrawal.");
                    //    PersonsHistory pHistoryObject = new PersonsHistory()
                    //    {
                    //        Bmsid = badgeStudent.StudentId,
                    //        LastName = badgeStudent.LastName,
                    //        FirstName = badgeStudent.FirstName,
                    //        MiddleName = badgeStudent.MiddleName,
                    //        SchoolName = badgeStudent.SchoolName,
                    //        SchoolNumber = badgeStudent.SchoolNumber,
                    //        Grade = badgeStudent.Grade,
                    //        MifareCardNumber = badgeStudent.MifareCardNumber,
                    //        MifareBusPassNum = badgeStudent.MifareBusPassNum,
                    //        MifareApproved = badgeStudent.MifareApproved,
                    //        MifareEligible = badgeStudent.MifareEligible,
                    //        TimeStamp = DateTime.Parse(x),
                    //        HashkeyExist = badgeStudent.Hashkey,
                    //        LastUpdatedBy = badgeStudent.LastUpdatedBy,
                    //        PrintDate = badgeStudent.PrintDate,
                    //        IssueDate = badgeStudent.IssueDate != null ? badgeStudent.IssueDate.Value.ToString("yyyy-MM-dd") : DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    //        BadgeType = badgeStudent.BadgeType
                    //    };
                    //    var resp = await _repo.PostPersonsHistoryToStudentHistoricals(pHistoryObject);
                    //    if (resp.Response.IsSuccessStatusCode)
                    //    {
                    //        _logger.LogInformation($"Successfully posted student-historicals object {pHistoryObject.Bmsid} for badge student withdrawal.");
                    //        LoggerLQ.LogQueue($"Successfully posted student-historicals object {pHistoryObject.Bmsid} for badge student withdrawal.");
                    //    }
                    //    else
                    //    {
                    //        _logger.LogInformation($"Failed to post student-historicals object {pHistoryObject.Bmsid} for badge student withdrawal");
                    //        LoggerLQ.LogQueue($"Failed to post student-historicals object {pHistoryObject.Bmsid} for badge student withdrawal.");
                    //    }
                    //}
                }
                return;
            }
            //Create record
            else if (badgeStudent == null && activeEdFiEnrollmentSchool != null)
            {
                _logger.LogInformation($"Creating badge for {edfiStudUniqueId}");

                badgeStudent = new BadgeStudent
                {
                    StudentId = edfiStudUniqueId,
                    LastName = edfiV3EnrollmentStudent.LastSurname,
                    FirstName = edfiV3EnrollmentStudent.FirstName,
                    MiddleName = edfiV3EnrollmentStudent.MiddleName,
                    BirthDate = edfiV3EnrollmentStudent.BirthDate,
                    SchoolNumber = enrollmentSchoolIDCode,
                    SchoolName = activeEdFiEnrollmentSchool.NameOfInstitution,
                    Grade = edfiGradeLevel,
                    Homeroom = homeroomCode,
                    ServType = servType,
                    Calendar = calendar,
                    LastUpdated = dateOnly,
                    PersonId = icStudentEnrollment.PersonId.ToString(),
                };

                //Set badge types
                if (badgeStudent.Grade == "P3" || badgeStudent.Grade == "P4" || badgeStudent.Grade == "K") { badgeStudent.BadgeType = "MNPS_Student_Badge_Non_Bus_Pass"; }
                else
                {
                    byte grade = Convert.ToByte(badgeStudent.Grade);
                    if (Enumerable.Range(9, 12).Contains(grade)) { badgeStudent.BadgeType = "MNPS_Student_Badge"; }
                    else if (Enumerable.Range(1, 8).Contains(grade)) { badgeStudent.BadgeType = "MNPS_Student_Badge_Non_Bus_Pass"; }
                }

                if (badgeStudent.SchoolNumber == "830" || icStudentEnrollment.CalendarName.Contains("Genesis") || icStudentEnrollment.CalendarName.Contains("High Road"))
                {
                    badgeStudent.SchoolName = activeEdFiEnrollmentSchool.NameOfInstitution;
                    badgeStudent.BadgeType = "MNPS_SPED_BADGE";
                }

                // V6 Docs 
                //if (string.IsNullOrEmpty(badgeStudent.MifareCardNumber) && badgeStudent.Expiration == null)
                //{
                //    var date = DateTime.Now.ToShortDateString(); var dateOnly = Convert.ToDateTime(date);
                //    badgeStudent.IssueDate = dateOnly;
                //}

                // V6 Docs - disregard for now
                //if (badgeStudent.MifareCardNumber == null)
                //{
                //    //Send record to MTA deactivation list
                //}

                if ((badgeStudent.SchoolName == "Harris-Hillman Special Education" || edfiSSA.SchoolReference.SchoolId == 1900302) && badgeStudent.Homeroom == "CBTP")
                {
                    //badgeStudent.SchoolName = "CBTP";
                    badgeStudent.BadgeType = "MNPS_SPED_BADGE";
                }

                var repoResponse = await _athen.CreateStudentBadge(badgeStudent);
                if (repoResponse.HttpRespMsg.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Created student badge {badgeStudent.StudentId}");
                    LoggerLQ.LogQueue($"Created student badge {badgeStudent.StudentId}");
                }
                else
                {
                    LoggerLQ.LogQueue($"Failed creating student badge {badgeStudent.StudentId}");

                    //LoggerLQ.TrackTelemetryEvent("Request Failure", new Dictionary<string, string> {
                    //{ "Description", $"Failed creating student badge {badgeStudent.StudentId}" },
                    //{ "Headers", repoResponse.Response.Headers.ToString() },
                    //{ "Request Message", repoResponse.Response.RequestMessage.ToString() },
                    //{ "Reason", repoResponse.Message }
                    //});
                    LoggerLQ.LogQueue($"Failed creating student badge {badgeStudent.StudentId}");
                }
                return;
            }
            //No records
            else if (badgeStudent == null && activeEdFiEnrollmentSchool == null)
            {
                _logger.LogInformation($"Null badge student {edfiStudUniqueId} and no active ed-fi enrollments.");
                return;
            }
            //-----------------------------------------------------------------------------------------
            _logger.LogInformation($"{edfiStudUniqueId} up to date");
        }



        public async Task UpdateStudentsFromIC(TypeOfSync typeOfSync)
        {
            string today = DateTime.Now.ToShortDateString();
            string studUniqId = "190313636";
            int pagesize = 1000;
            int offset = 0;
            const int limit = 100;
            bool isFinished = false;
            int batchNumber = 1;
            var tasks = new List<Task>();

            //var newICStudentEnrollment = await _athen.GetNewICStudentEnrollments();

            do
            {
                if(typeOfSync == TypeOfSync.EnrollmtChanges)
                {
                    _logger.LogInformation($"Getting IC student enrollment changes, offset {offset}, pagesize {pagesize}");
                    List<ICEnrollmentChange> icEnrChangeList = new List<ICEnrollmentChange>();
                    HttpResponse<List<ICEnrollmentChange>> enrChangesResponse = new HttpResponse<List<ICEnrollmentChange>>();
                    enrChangesResponse = await _athen.GetICEnrollmentChanges(offset, pagesize);
                    if (!enrChangesResponse.IsSuccess)
                    {
                        _logger.LogInformation($"Failed getting IC enrollment changes on offset {offset}");
                        LoggerLQ.LogQueue($"Failed getting IC enrollment changes on offset {offset}");
                        break;
                    }
                    if(enrChangesResponse.ResponseContent != null)
                    {
                        icEnrChangeList = enrChangesResponse.ResponseContent;
                    }
                    else
                    {
                        _logger.LogInformation($"Null enrChangesResponse.ResponseContent at GetICEnrollmentChanges({offset}, {pagesize}");
                        LoggerLQ.LogQueue($"Null enrChangesResponse.ResponseContent at GetICEnrollmentChanges({offset}, {pagesize}");
                    }
                    if(icEnrChangeList.Count > 0)
                    {
                        foreach(var icEnrChange in icEnrChangeList)
                        {
                            try
                            {
                                var result = RunEdfiOdsSync(icEnrChange);
                                if (result != null)
                                {
                                    tasks.Add(result);
                                }
                                else
                                {
                                    _logger.LogInformation($"Null result at RunEdfiOdsSync()");
                                    LoggerLQ.LogQueue($"Null result at RunEdfiOdsSync()");
                                }
                            }
                            catch (Exception exc)
                            {
                                _logger.LogInformation($"Exception in RunEdfiOdsSync() for enrollment-changes; {exc.Message}");
                                throw;
                            }
                        }
                    }
                    batchNumber++;
                    offset += icEnrChangeList.Count;
                    if(icEnrChangeList.Count == 0)
                    {
                        isFinished = true;
                    }
                }
                if (typeOfSync == TypeOfSync.SingleEnrStudent)
                {
                    //GET SINGLE IC STUDENT ENR
                    HttpResponse<ICStudentEnrollment> icStdEnrollmentResult = new HttpResponse<ICStudentEnrollment>();
                    _logger.LogInformation($"Getting next IC student enrollment {today}, offset {offset}");
                    icStdEnrollmentResult = await _athen.GetICStudentEnrollmentByStudentNumber(studUniqId);
                    if (!icStdEnrollmentResult.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"Failed getting next IC student enrollment {studUniqId} on offset {offset}");
                        LoggerLQ.LogQueue($"Failed getting next IC student enrollment {studUniqId} on offset {offset}");
                        break;
                    }
                    ICStudentEnrollment icStdEnrmt = icStdEnrollmentResult.ResponseContent;
                    var result = RunEdfiOdsSync(icStdEnrmt);
                    tasks.Add(result);
                    isFinished = true; 
                }

                //Await all tasks
                try
                {
                    Task batchTask = Task.WhenAll(tasks);
                    Console.WriteLine("Main thread done");
                    batchTask.Wait();
                }
                catch (Exception e)
                {
                    //_logger.LogInformation($"Error in synchronizing batch {batchNumber} of enrollments. {e.Message}");
                    LoggerLQ.LogQueue($"Error in synchronizing batch {batchNumber} - {e.Message}");
                }
            } while (!isFinished);
        }
    }
}
