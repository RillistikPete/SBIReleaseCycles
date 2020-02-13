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
        public Athenaeum(IEdfiClient edfiClient, EdfiClientCompositeBase edfiClientComposite, IICClient iCClient)
        {
            _edfiClient = edfiClient;
            _edfiClientComposite = edfiClientComposite;
            _icClient = iCClient;
        }

        //Implement methods from IAthenaeum
        Task<HttpResponse<List<ICStudentEnrollment>>> GetICStudentEnrollment(string studentId)
        {
        }
    }
}
