using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StdBdgRCCL.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StdBdgRCCL.Infrastructure
{
    public partial class Updater : IUpdater
    {
        private readonly IAthenaeum _athen;
        private readonly ILogger _logger;
        //private readonly HttpClient _edfiClient;
        //private readonly HttpClient _edfiClientComposite;
        //private readonly HttpClient _icClient;
        //private readonly IHttpClientFactory httpClientFactory;


        public Updater(IAthenaeum athenaeum, ILogger logger)
        {
            _athen = athenaeum;
            _logger = logger;
            //_edfiClient = httpClientFactory.CreateClient("edfiClient");
            //_icClient = httpClientFactory.CreateClient("icClient");
            //_edfiClientComposite = httpClientFactory.CreateClient("edfiClientComposite");
        }

        public async Task UpdateStudents()
        {
            //_logger.LogInformation("Checking for updates to the ODS");
            await UpdateStudentsFromIC();
        }
    }
}
