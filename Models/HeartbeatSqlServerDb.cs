using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartbeatService.Models
{
    public class HeartbeatSqlServerDb
    {
        public string DbName { get; set; }

        public string Server { get; set; }

        public string ConnectionString { get; set; }
    }
}
