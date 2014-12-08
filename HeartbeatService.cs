using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using HeartbeatService.Models;
using Newtonsoft.Json;
using System.IO;
using HeartbeatService.Services;
using System.Reflection;
using HeartbeatService.Utils;

namespace HeartbeatService
{
    partial class HeartbeatService : ServiceBase
    {
        private ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private Thread _thread;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public HeartbeatService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Debug method required to test service using infinite thread
        /// </summary>
        public void Start()
        {
            OnStart(new string[] { });
        }

        protected override void OnStart(string[] args)
        {
            logger.Info("Heartbeat service starting");
            _thread = new Thread(WorkerThreadFunc);
            _thread.Name = "Heartbeat Service Worker";
            _thread.IsBackground = true;
            _thread.Start();
        }

        protected override void OnStop()
        {
            _shutdownEvent.Set();

            // give the thread 3 seconds to stop
            if (!_thread.Join(3000))
            {
                _thread.Abort();
            }
            logger.Info("Heartbeat service stopped");

        }

        string ConfigPath
        {
            get
            {
                return Path.Combine(PathUtils.AppFolder(), "Heartbeat.config.json");
            }
        }

        void WorkerThreadFunc()
        {
            HeartbeatConfig config = null;
            try
            {
                // load the config
                string json = File.ReadAllText(ConfigPath);
                config = JsonConvert.DeserializeObject<HeartbeatConfig>(json);
            }
            catch (Exception ex)
            {
                logger.Fatal(String.Format("Heartbeat service failed - invalid config: {0}", ConfigPath), ex);
                Environment.Exit(1);
            }

            while (!_shutdownEvent.WaitOne(0))
            {
                // work out the next run time - if we don't find one in the config then we use the earliest one and adjust for tomorrow
                TimeService timeService = new TimeService(config);
                DateTime nextRunTime = timeService.GetNextRunTime();
                int sleepTime = timeService.GetMillisecondsUntil(nextRunTime);
                logger.Info("Next run time: {0} ({1} millisconds from now)", nextRunTime, sleepTime);
                Thread.Sleep(sleepTime);

                try
                {
                    logger.Info("Running heartbeat checks");

                    // run the check
                    StatusCheckService checker = new StatusCheckService(config);
                    List<HeartbeatResult> result = checker.CheckAll();

                    // generate and send off the mail
                    MailBuilderService mailBuilder = new MailBuilderService();
                    string mailBody = mailBuilder.BuildMail(result);

                    MailService mailService = new MailService(config);
                    mailService.SendMail(config.Mail.ToAddresses, config.Mail.Subject, mailBody);

                    logger.Info("Heartbeat checks complete");
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message, ex);
                    Thread.Sleep(1000 * 60 * 5);     // 5 minutes
                }
            }

        }
    }
}
