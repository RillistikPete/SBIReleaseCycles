using StdBdgRCCL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StdBdgRCCL.Infrastructure
{
    public partial class Updater : IUpdater
    {
        private readonly IAthenaeum _athen;
        //private readonly ILogger _logger;

        public Updater(IAthenaeum athenaeum)
        {
            _athen = athenaeum;
        }

        public async Task UpdateStudents()
        {
            //_logger.LogInformation("Checking for updates to the ODS");
            await UpdateStudentsFromIC();
        }
    }
}
