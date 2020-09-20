using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MailDatabase.Models
{
    public class Mailbox
    {
        [Column(Order = 0)]
        public string MailboxName { get; set; }

        [Column(Order = 1)]
        public TimeSpan TrashTimer { get; set; }

        public Mailbox() { }

        public Mailbox(string mailboxName)
        {
            MailboxName = mailboxName;
            TrashTimer = TimeSpan.FromDays(30);
        }
    }
}
