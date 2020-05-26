using System.Collections.Generic;
using System.Linq;
namespace MailDatabase.LetterTypes
{
    public class SentLetters : ILetterType
    {
        public IEnumerable<int> GetLettersFromFolder(string mailbox)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                return db.MailboxesToMails
                    .Where(x => x.MailboxName == mailbox && x.FolderId == 1)
                    .Select(x => x.MailId);
            }
        }
    }
}
