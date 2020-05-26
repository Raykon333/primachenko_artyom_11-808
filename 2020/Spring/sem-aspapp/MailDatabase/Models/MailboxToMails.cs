using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDatabase.Models
{
    class MailboxToMails
    {
        [Column(Order = 0)]
        public string MailboxName { get; internal set; }

        [Column(Order = 1)]
        public int MailId { get; internal set; }

        [Column(Order = 2)]
        public int FolderId { get; internal set; }

        public MailboxToMails() { }

        public MailboxToMails(string mailboxName, int mailId, int folderId)
        {
            MailboxName = mailboxName;
            MailId = mailId;
            FolderId = folderId;
        }
    }
}
