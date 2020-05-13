using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MailDatabase
{
    class MailboxToMails
    {
        [Column(Order = 0)]
        public string MailboxName { get; internal set; }

        [Column(Order = 1)]
        public int MailId { get; internal set; }

        public MailboxToMails() { }

        public MailboxToMails(string mailboxName, int mailId)
        {
            MailboxName = mailboxName;
            MailId = mailId;
        }
    }
}
