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
        /// Gets the IC Student Enrollment resource based on the student id
        /// </summary>
        /// <param name="studentId">Student Number (190xxxxxx)</param>
        /// <returns></returns>
        Task<HttpResponse<List<ICStudentEnrollment>>> GetICStudentEnrollment(string studentId);
    }
}
