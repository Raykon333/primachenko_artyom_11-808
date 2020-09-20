using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;

namespace GachiMail
{
    internal class ScheduledTasks
    {
        IDatabaseService db;

        internal ScheduledTasks(IDatabaseService database)
        {
            db = database;
        }

        public void UpdateTierLevels()
        {
            db.UpdateTierForAll();
            BackgroundJob.Schedule(() => UpdateTierLevels(),
                TimeSpan.FromSeconds(11));
        }

        public void ClearTrash()
        {
            db.DeleteAllTimedOutTrash();
            BackgroundJob.Schedule(() => ClearTrash(),
                DateTime.Today.AddDays(1) - DateTime.Now);
        }
    }
}
