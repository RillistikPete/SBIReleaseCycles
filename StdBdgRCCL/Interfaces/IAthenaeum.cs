using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using StdBdgRCCL.Models;

namespace StdBdgRCCL.Interfaces
{
    public interface IAthenaeum
    {
        /// <summary>
        /// Gets latest IC Enrollment Changes
        /// </summary>
        /// <returns></returns>
        Task<HttpResponse<List<ICEnrollmentChange>>> GetICEnrollmentChanges(int offset, int pagesize);

        /// <summary>
        /// Gets the IC Student Enrollment resource based on the student id
        /// </summary>
        /// <param name="studentId">Student Number (190xxxxxx)</param>
        /// <returns></returns>
        Task<HttpResponse<ICStudentEnrollment>> GetICStudentEnrollmentByStudentNumber(string studentId);

        /// <summary>
        /// Gets the IC Student Enrollment resource based on the personid
        /// </summary>
        /// <param name="personID">PersonID</param>
        /// <returns></returns>
        Task<HttpResponse<ICStudentEnrollment>> GetICStudentEnrollmentByPersonId(string personID);

        /// <summary>
        /// Gets an EdFi Enrollment Student specified by id.
        /// </summary>
        /// <param name="id">Guid id of student</param>
        /// <returns>EdFiEnrollmentStudent Object</returns>
        Task<HttpResponse<EdfiEnrollmentStudent>> GetEdFiEnrollmentStudentByStudentUniqueId(string stdUniqueId, IDictionary<string, string> properties);

        /// <summary>
        /// Gets records from StudentSchoolAssociations resource by studentUniqueId
        /// </summary>
        /// <param name="studentUniqueId"></param>
        /// <returns></returns>
        Task<HttpResponse<List<StudentSchoolAssociation>>> GetStudentSchoolAssociationsByStudentUniqueId(string studentUniqueId, IDictionary<string, string> properties);

        /// <summary>
        /// Gets an EdFi Enrollment School specified by id.
        /// </summary>
        /// <param name="id">Guid id of school</param>
        /// <returns>EdFiEnrollmentSchool Object</returns>
        Task<HttpResponse<EdfiEnrollmentSchool>> GetEdFiEnrollmentSchoolById(long id);

        //CALENDAR
        /// <summary>
        /// Gets an EdFi Calendar filtered by schoolId.
        /// </summary>
        /// <param name="schoolId">schoolId of student's active school</param>
        /// <returns> Calendar </returns>
        Task<HttpResponse<Models.Calendar>> GetCalendarBySchoolId(int schoolId, IDictionary<string, string> properties);

        //HOMEROOM
        /// <summary>
        /// Gets an EdFi Location filtered by schoolId.
        /// </summary>
        /// <param name="schoolId">schoolId of student's active school</param>
        /// <returns> List<Location> </returns>
        Task<HttpResponse<List<Location>>> GetLocationsBySchoolId(int schoolId, IDictionary<string, string> properties);

        /// <summary>
        ///  Gets a single record from the Badge System Student Badges resource by specified Id.
        /// </summary>
        /// <param name="id">Badge bmsid</param>
        /// <returns>
        ///  List of Badge System Students
        /// </returns>
        Task<HttpResponse<List<BadgeStudent>>> GetBadgeSystemStudentById(string id);


        /// <summary>
        /// Gets records from StudentSchedule resource by studentUniqueId
        /// </summary>
        /// <param name="studentUniqueId"></param>
        /// <returns>RepoResponse<List<StudentSchedule>></returns>
        Task<HttpResponse<List<StudentSchedule>>> GetStudentScheduleById(string studentId);


        //=================================CRUD=================================================

        /// <summary>
        /// Updates Student Badge Record in the Badge System
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <param name="dto">Student Badge DTO</param>
        /// <returns></returns>
        Task<ServerResponse> UpdateStudentBadge(string id, BadgeStudent dto);

        /// <summary>
        /// Creates a new Student Badge Record in the Badge System
        /// </summary>
        /// <param name="badgeStudent"></param>
        /// <returns></returns>
        Task<ServerResponse> CreateStudentBadge(BadgeStudent badgeStudent);


    }
}
