using StdBdgRCCL.Infrastructure;
using StdBdgRCCL.Interfaces;
using StdBdgRCCL.Models;
using System.Threading.Tasks;

namespace StdBdgRCCL
{
    public partial class Updater
    {
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

    }
}
