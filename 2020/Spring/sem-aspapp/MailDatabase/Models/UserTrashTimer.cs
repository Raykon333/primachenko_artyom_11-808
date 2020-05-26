using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDatabase
{
    class UserTrashTimer
    {
        [Column(Order = 0), MaxLength(20)]
        public string UserLogin { get; internal set; }

        public TimeSpan TrashTimer { get; internal set; }

        public UserTrashTimer() { }

        public UserTrashTimer(string userLogin, TimeSpan trashTimer)
        {
            UserLogin = userLogin;
            TrashTimer = trashTimer;
        }
    }
}
