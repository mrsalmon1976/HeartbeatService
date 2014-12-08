using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartbeatService.Models
{
    public class HeartbeatMail
    {
        public int Port { get; set; }

        public string Server { get; set; }

        public string FromAddress { get; set; }

        public string[] ToAddresses { get; set; }

        public string Subject { get; set; }
    }
}
