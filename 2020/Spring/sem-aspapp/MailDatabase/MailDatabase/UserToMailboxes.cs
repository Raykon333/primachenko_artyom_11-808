using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDatabase
{
    class UserToMailboxes
    {
        [Column(Order = 0)]
        public string UserLogin { get; internal set; }

        [Column(Order = 1)]
        public string MailboxName { get; internal set; }

        public UserToMailboxes() { }

        public UserToMailboxes(string userLogin, string mailboxName)
        {
            UserLogin = userLogin;
            MailboxName = mailboxName;
        }
    }
}
