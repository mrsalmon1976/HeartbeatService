using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartbeatService.Models
{
    public class HeartbeatResult
    {
        public HeartbeatResult(string serviceType, string name)
        {
            this.IsAlive = false;
            this.ServiceType = serviceType;
            this.Name = name;
        }

        public bool IsAlive { get; set; }

        public string Name { get; set; }

        public string ServiceType { get; set; }

    }
}
