using StdBdgRCCL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StdBdgRCCL.Infrastructure;

namespace StdBdgRCCL
{
    public partial class Updater
    {
        //public async Task<HttpResponse<BadgeStudent>> GetBadgeStudentAsync(string studentId)
        //{
        //    var badgeStudentResponse = await _athen.GetBadgeSystemStudentById(studentId);
        //    if (!badgeStudentResponse.IsSuccessStatusCode)
        //    {
        //        return new HttpResponse<BadgeStudent> { IsSuccess = false };
        //    }
        //    if (badgeStudentResponse.Content.Count == 0)
        //    {
        //        return new HttpResponse<BadgeStudent> { IsSuccess = true, Content = null };
        //    }
        //    else
        //    {
        //        return new HttpResponse<BadgeStudent> { IsSuccess = true, Content = badgeStudentResponse.Content[0] };
        //    }
        //}

    }
}
