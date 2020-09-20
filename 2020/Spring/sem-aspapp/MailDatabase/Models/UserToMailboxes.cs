using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDatabase.Models
{
    public class UserToMailboxes
    {
        [Column(Order = 0)]
        public string UserLogin { get; set; }

        [Column(Order = 1)]
        public string MailboxName { get; set; }

        public UserToMailboxes() { }

        public UserToMailboxes(string userLogin, string mailboxName)
        {
            UserLogin = userLogin;
            MailboxName = mailboxName;
        }
    }
}
