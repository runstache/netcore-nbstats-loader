using System;
using System.Collections.Generic;
using System.Text;

namespace NbaStats.Loader.Configuration
{
    public class AppSettings
    {
        public string ConnectionString { get; set; }
        public string ImportDirectory { get; set; }
        public bool ScheduleOnly { get; set; }
        public string LogFile { get; set; }
        public bool RostersOnly { get; set; }
    }
}
