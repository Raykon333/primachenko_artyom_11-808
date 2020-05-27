using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDatabase.Models
{
    class Mailbox
    {
        [Column(Order = 0)]
        public string MailboxName { get; internal set; }

        [Column(Order = 1)]
        public TimeSpan TrashTimer { get; internal set; }

        public Mailbox() { }

        public Mailbox(string mailboxName)
        {
            MailboxName = mailboxName;
            TrashTimer = TimeSpan.FromDays(30);
        }
    }
}
