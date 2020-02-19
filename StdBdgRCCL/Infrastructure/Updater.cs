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
        public enum TypeOfSync { EnrollmtChanges, SingleEnrStudent, SchoolAssociations, SpecialConditn }

        public Updater(IAthenaeum athenaeum, ILogger logger)
        {
            _athen = athenaeum;
            _logger = logger;
        }

        public async Task UpdateStudents()
        {
            _logger.LogInformation("Checking for updates in IC");
            await UpdateStudentsFromIC(TypeOfSync.EnrollmtChanges);
        }
    }
}
