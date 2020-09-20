using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDatabase.Models
{
    public class MailboxToMails
    {
        [Column(Order = 0)]
        public string MailboxName { get; set; }

        [Column(Order = 1)]
        public int MailId { get; set; }

        [Column(Order = 2)]
        public int FolderId { get; set; }

        public MailboxToMails() { }

        public MailboxToMails(string mailboxName, int mailId, int folderId)
        {
            MailboxName = mailboxName;
            MailId = mailId;
            FolderId = folderId;
        }
    }
}
