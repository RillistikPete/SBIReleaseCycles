using Microsoft.EntityFrameworkCore;
using StdBdgRCCL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StdBdgRCCL.Infrastructure
{
    public class LoggingContext : DbContext
    {
        public LoggingContext(DbContextOptions<LoggingContext> options) : base(options)
        {
            
        }

        public DbSet<Log> Logs { get; set; }
    }
}
