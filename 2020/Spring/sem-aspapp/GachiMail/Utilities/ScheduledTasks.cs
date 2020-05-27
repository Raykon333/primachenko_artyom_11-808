using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;

namespace GachiMail
{
    internal class ScheduledTasks
    {
        public static void UpdateTierLevels()
        {
            MailDatabase.DatabaseOperations.UpdateTierForAll();
            BackgroundJob.Schedule(() => UpdateTierLevels(),
                DateTime.Today.AddDays(1) - DateTime.Now);
        }

        public static void ClearTrash()
        {
            MailDatabase.DatabaseOperations.DeleteAllTimedOutTrash();
            BackgroundJob.Schedule(() => ClearTrash(),
                DateTime.Today.AddDays(1) - DateTime.Now);
        }
    }
}
