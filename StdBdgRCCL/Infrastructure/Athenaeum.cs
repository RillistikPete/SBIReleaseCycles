using StdBdgRCCL.Infrastructure.ClientBase;
using StdBdgRCCL.Interfaces;
using StdBdgRCCL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StdBdgRCCL.Infrastructure
{
    public class Athenaeum : IAthenaeum
    {
        private readonly IEdfiClient _edfiClient;
        private readonly EdfiClientCompositeBase _edfiClientComposite;
        private readonly IICClient _icClient;
        private readonly IICClient _badgeClient;
        public Athenaeum(IEdfiClient edfiClient, EdfiClientCompositeBase edfiClientComposite, IICClient iCClient, IICClient badgeClient)
        {
            _edfiClient = edfiClient;
            _edfiClientComposite = edfiClientComposite;
            _icClient = iCClient;
            _badgeClient = badgeClient;
        }


        #region Implementation

        public async Task<HttpResponse<List<ICEnrollmentChange>>> GetICEnrollmentChanges(int offset, int pagesize)
        {
            return await _icClient.GetByExample<ICEnrollmentChange>($"enrollment-changes?offset={offset}&pagesize={pagesize}");
        }

        public async Task<HttpResponse<ICStudentEnrollment>> GetICStudentEnrollmentByStudentNumber(string studentId)
        {
            return await _icClient.GetSingleByExample<ICStudentEnrollment>($"studentEnrollments?sysfilter=equal(studentNumber: \"{studentId}\") ", null);
        }

        public async Task<HttpResponse<ICStudentEnrollment>> GetICStudentEnrollmentByPersonId(string personID)
        {
            return await _icClient.GetSingleByExample<ICStudentEnrollment>($"studentEnrollments?sysfilter=equal(personID: \"{personID}\") ", null);
        }

        public async Task<HttpResponse<EdfiEnrollmentStudent>> GetEdFiEnrollmentStudentByStudentUniqueId(string stdUniqueId, IDictionary<string, string> properties)
        {
            return await _edfiClientComposite.GetSingleByExample<EdfiEnrollmentStudent>($"enrollment/students?studentUniqueId={stdUniqueId}", properties);
        }

        public async Task<HttpResponse<List<StudentSchoolAssociation>>> GetStudentSchoolAssociationsByStudentUniqueId(string studentUniqueId, IDictionary<string, string> properties)
        {
            return await _edfiClient.GetByExample<StudentSchoolAssociation>($"studentSchoolAssociations?studentUniqueId={studentUniqueId}", properties);
        }

        public async Task<HttpResponse<EdfiEnrollmentSchool>> GetEdFiEnrollmentSchoolById(long schoolId)
        {
            return await _edfiClientComposite.GetSingle<EdfiEnrollmentSchool>($"enrollment/schools?schoolId={schoolId}");
        }

        //Calendar
        public async Task<HttpResponse<Models.Calendar>> GetCalendarBySchoolId(int schoolId, IDictionary<string, string> properties)
        {
            return await _edfiClient.GetSingleByExample<Models.Calendar>($"Calendars?schoolId={schoolId}", properties);
        }

        //HOMEROOM
        public async Task<HttpResponse<List<Location>>> GetLocationsBySchoolId(int schoolId, IDictionary<string, string> properties)
        {
            return await _edfiClient.GetByExample<Location>($"Locations?schoolId={schoolId}", properties);
        }

        public async Task<HttpResponse<List<BadgeStudent>>> GetBadgeSystemStudentById(string id)
        {
            return await _badgeClient.GetById<BadgeStudent>("studentBadges/", id);
        }

        public async Task<HttpResponse<List<StudentSchedule>>> GetStudentScheduleById(string studentId)
        {
            //return await _icClient.GetByExample<StudentSchedule>($"studentSchedules?filter=\"studentNumber\"='{studentId}'&order=\"term\",\"periodSequence\",\"scheduleSequence\"", null);
            //return await _icClient.GetByExample<StudentSchedule>($"studentSchedules?sysfilter=equal(studentNumber:'{studentId}')", null);
            return await _icClient.GetByExample<StudentSchedule>($"studentSchedules?filter=studentNumber='{studentId}'&order='term','periodSequence','scheduleSequence'", null);
        }
        #endregion

        #region CRUD

        public async Task<ServerResponse> UpdateStudentBadge(string id, BadgeStudent dto)
        {
            return await _badgeClient.Put("studentBadges/", id, dto);
        }

        public async Task<ServerResponse> CreateStudentBadge(BadgeStudent badgeStudent)
        {
            return await _badgeClient.Post("studentBadges/", badgeStudent);
        }
        #endregion

    }
}
