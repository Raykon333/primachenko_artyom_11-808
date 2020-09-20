/*using System;
using System.Collections.Generic;
using System.Linq;
namespace MailDatabase.LetterTypes
{
    public class IncomingLetters : ILetterType
    {
        public IEnumerable<int> GetLettersFromFolder(string mailbox)
        {
            var answ = new List<int>();
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                var list = db.MailboxesToMails
                    .Where(x => x.MailboxName == mailbox && x.FolderId == 0)
                    .Select(x => x.MailId);
                foreach (var i in list)
                    answ.Add(i);
            }
            return answ;
        }
    }
}*/
