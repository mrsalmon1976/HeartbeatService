using HeartbeatService.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartbeatService.Services
{
    public class TimeService
    {
        private HeartbeatConfig config = null;

        public TimeService(HeartbeatConfig config)
        {
            this.config = config;
        }

        public DateTime GetNextRunTime()
        {
            DateTime fromDate = DateTime.Now;

            // work out the next run time - if we don't find one in the config then we use the earliest one and adjust for tomorrow
            foreach (TimeSpan ts in config.RunTimes.OrderBy(x => x.Hours))
            {
                DateTime nextRunTime = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, ts.Hours, ts.Minutes, 0);
                if (nextRunTime > fromDate) return nextRunTime;
            }

            // no date, let's work out the first date tomorrow
            fromDate = fromDate.AddDays(1);
            TimeSpan tst = config.RunTimes.OrderBy(x => x.Hours).First();
            return new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, tst.Hours, tst.Minutes, 0);

        }

        public int GetMillisecondsUntil(DateTime dt)
        {
            return Convert.ToInt32(dt.Subtract(DateTime.Now).TotalMilliseconds);
        }

    }
}
