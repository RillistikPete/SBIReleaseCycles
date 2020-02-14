using Microsoft.EntityFrameworkCore;
using StdBdgRCCL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace StdBdgRCCL.Models
{
    public static class Logger
    {
        private static object _loggerLock = new object();

        public static void Log(string message, string stack = "")
        {
            lock (_loggerLock)
            {
                var optionsBuilder = new DbContextOptionsBuilder<LoggingContext>();
                optionsBuilder.UseSqlServer("LoggingDb");
                using (var db = new LoggingContext(optionsBuilder.Options))
                {
                    //TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                    //DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now, cstZone);
                    DateTime cstTime = DateTime.Today;
                    var entry = new Log
                    {
                        RowAdded = cstTime,
                        AppName = "StudentBadgeReleaseCycles",
                        Category = "Event",
                        LoggedBy = "gsAzure",
                        Message = message,
                        SeverityLevel = 0,
                        StackTrace = stack
                    };
                    db.Logs.Add(entry);
                    db.SaveChanges();
                }
            }
        }
    }
}
