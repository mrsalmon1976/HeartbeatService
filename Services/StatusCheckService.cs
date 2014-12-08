using HeartbeatService.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Management;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace HeartbeatService.Services
{
    public class StatusCheckService
    {
        private HeartbeatConfig config = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public StatusCheckService(HeartbeatConfig config)
        {
            this.config = config;
        }

        public List<HeartbeatResult> CheckAll()
        {
            List<HeartbeatResult> result = new List<HeartbeatResult>();

            result.AddRange(this.CheckDatabases());
            result.AddRange(this.CheckWebsites());
            result.AddRange(this.CheckWindowsServices());

            return result;
        }

        public List<HeartbeatResult> CheckDatabases()
        {
            List<HeartbeatResult> result = new List<HeartbeatResult>();

            foreach (HeartbeatSqlServerDb db in this.config.SqlServerDatabases)
            {
                HeartbeatResult dbResult = new HeartbeatResult("Database", String.Format("{0}\\{1}", db.Server, db.DbName));

                try
                {
                    using (SqlConnection conn = new SqlConnection(db.ConnectionString))
                    {
                        conn.Open();
                        dbResult.IsAlive = (conn.State == ConnectionState.Open);
                    }
                }
                catch (Exception ex)
                {
                    logger.Info(ex.Message, ex);
                    dbResult.IsAlive = false;
                }

                result.Add(dbResult);
            }

            return result;
        }

        public List<HeartbeatResult> CheckWebsites()
        {
            List<HeartbeatResult> result = new List<HeartbeatResult>();

            foreach (HeartbeatWebSite site in this.config.Websites)
            {
                HeartbeatResult siteResult = new HeartbeatResult("Web", site.Url);

                try
                {
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(site.Url);
                    webRequest.AllowAutoRedirect = false;
                    using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
                    {
                        siteResult.IsAlive = (response.StatusCode == HttpStatusCode.OK);
                    }
                }
                catch (Exception ex)
                {
                    logger.Info(ex.Message, ex);
                    siteResult.IsAlive = false;
                }

                result.Add(siteResult);
            }

            return result;
        }

        public List<HeartbeatResult> CheckWindowsServices()
        {
            List<HeartbeatResult> result = new List<HeartbeatResult>();

            foreach (HeartbeatWindowsService service in this.config.WindowsServices)
            {
                HeartbeatResult serviceResult = new HeartbeatResult("Windows Service", service.Name);

                try
                {
                    ServiceController sc = new ServiceController(service.Name, service.Server);
                    serviceResult.IsAlive = (sc.Status == ServiceControllerStatus.Running);
                }
                catch (Exception ex)
                {
                    logger.Info(ex.Message, ex);
                    serviceResult.IsAlive = false;
                }

                result.Add(serviceResult);
            }

            return result;
        }
    }
}
