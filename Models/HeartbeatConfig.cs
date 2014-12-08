using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartbeatService.Models
{
    public class HeartbeatConfig
    {
        public TimeSpan[] RunTimes { get; set; }
        public HeartbeatMail Mail { get; set; }
        public HeartbeatWebSite[] Websites { get; set; }
        public HeartbeatWindowsService[] WindowsServices { get; set; }
        public HeartbeatSqlServerDb[] SqlServerDatabases { get; set; }
    }
}
