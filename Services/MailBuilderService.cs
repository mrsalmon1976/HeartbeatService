using HeartbeatService.Models;
using HeartbeatService.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeartbeatService.Services
{
    public class MailBuilderService
    {
        private const string RowHtml = "<tr";

        private readonly string MailTemplateFilePath = String.Empty;
        private readonly string MailRowTemplateFilePath = String.Empty;

        public MailBuilderService()
        {
            string dir = PathUtils.AppFolder();
            MailTemplateFilePath = Path.Combine(dir, "Templates\\MailTemplate.html");
            MailRowTemplateFilePath = Path.Combine(dir, "Templates\\MailRowTemplate.html");
        }

        public string BuildMail(IEnumerable<HeartbeatResult> results)
        {
            string html = File.ReadAllText(MailTemplateFilePath);
            string row = File.ReadAllText(MailRowTemplateFilePath);

            StringBuilder sb = new StringBuilder();
            foreach (HeartbeatResult r in results)
            {
                string rh = row.Replace("{TYPE}", r.ServiceType)
                    .Replace("{NAME}", r.Name)
                    .Replace("{STATUS}", r.IsAlive ? "Alive" : "Error")
                    .Replace("{STATUS_COLOUR}", r.IsAlive ? "green" : "red");
                sb.Append(rh);
            }

            return html.Replace("{RESULTS}", sb.ToString());

        }
    }
}
