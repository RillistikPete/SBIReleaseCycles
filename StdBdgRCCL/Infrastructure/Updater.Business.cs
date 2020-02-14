using StdBdgRCCL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StdBdgRCCL.Infrastructure
{
    public partial class Updater
    {
        public async Task RunEdfiOdsSync(ICStudentEnrollment icStudentEnrollment)
        {

        }

        public async Task UpdateStudentsFromIC()
        {
            string today = DateTime.Now.ToShortDateString();
            string studUniqId = "190007507";
            int offset = 0;
            const int limit = 100;
            bool isFinished = false;
            int batchNumber = 1;
            var tasks = new List<Task>();

            //var newICStudentEnrollment = await _athen.GetNewICStudentEnrollments();

            do
            {
                //GET SINGLE IC STUDENT ENR
                HttpResponse<ICStudentEnrollment> icStdEnrollmentResult = new HttpResponse<ICStudentEnrollment>();
                //_logger.LogInformation($"Getting next batch of ed-fi enrollments {today}, offset {offset}");
                icStdEnrollmentResult = await _athen.GetICStudentEnrollmentByStudentNumber(studUniqId);
                if (!icStdEnrollmentResult.IsSuccessStatusCode)
                {
                    //_logger.LogInformation($"Failed getting next batch of EdFi Student School Associations on offset {offset}");
                    Logger.Log($"Failed getting IC Enrollment Student {studUniqId} on offset {offset}");
                    break;
                }
                ICStudentEnrollment icStdEnrmt = icStdEnrollmentResult.ResponseContent;
                var result = RunEdfiOdsSync(icStdEnrmt);
                tasks.Add(result);
                isFinished = true;

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
                    Logger.Log($"Error in synchronizing batch {batchNumber} - {e.Message}");
                }
            } while (!isFinished);
        }
    }
}
