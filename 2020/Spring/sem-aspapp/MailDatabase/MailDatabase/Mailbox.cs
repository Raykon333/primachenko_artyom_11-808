using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDatabase
{
    class Mailbox
    {
        [Column(Order = 0)]
        public string MailboxName { get; internal set; }

        public Mailbox() { }

        public Mailbox(string mailboxName)
        {
            MailboxName = mailboxName;
        }
    }
}
