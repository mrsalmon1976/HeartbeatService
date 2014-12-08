using HeartbeatService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace HeartbeatService.Services
{
    public class MailService
    {
        private HeartbeatConfig config = null;

        public MailService(HeartbeatConfig config)
        {
            this.config = config;
        }

        public void SendMail(string[] to, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient();
            client.Port = config.Mail.Port;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = config.Mail.Server;
            foreach (string t in to)
            {
                mail.To.Add(t);
            }
            mail.From = new MailAddress(config.Mail.FromAddress);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            client.Send(mail);
        }
    }
}
