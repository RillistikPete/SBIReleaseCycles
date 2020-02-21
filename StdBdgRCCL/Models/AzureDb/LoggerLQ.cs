using Microsoft.EntityFrameworkCore;
using StdBdgRCCL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace StdBdgRCCL.Models.AzureDb
{
    public class LoggerLQ
    {
        public static object LockObject = new object();
        public static void LogQueue(string message)
        {
            lock (LockObject)
            {
                var optionsBuilder = new DbContextOptionsBuilder<LoggingContext>();
                optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("LoggingDb"));
                using (var db = new LoggingContext(optionsBuilder.Options))
                {
                    DateTime cstTime = DateTime.Now;
                    LogQueue lq = new LogQueue
                    {
                        DateAdded = cstTime,
                        Message = message,
                        ProjectName = "StdBadgeReleaseCycles",
                        Logger = "EdfiDev"
                    };
                    db.LogQueue.Add(lq);
                    db.SaveChanges();
                }
            }
        }
    }
}
