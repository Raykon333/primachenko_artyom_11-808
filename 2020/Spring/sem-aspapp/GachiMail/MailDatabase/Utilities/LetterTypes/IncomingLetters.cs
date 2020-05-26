using System;
using System.Collections.Generic;
using System.Linq;
namespace MailDatabase.LetterTypes
{
    public class IncomingLetters : ILetterType
    {
        public IEnumerable<int> GetLettersFromFolder(string mailbox)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                return db.MailboxesToMails
                    .Where(x => x.MailboxName == mailbox && x.FolderId == 0)
                    .Select(x => x.MailId);
            }
        }
    }
}
