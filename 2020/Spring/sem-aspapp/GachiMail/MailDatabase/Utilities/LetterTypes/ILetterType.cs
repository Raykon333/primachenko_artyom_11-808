using System;
using System.Collections.Generic;
using System.Text;

namespace MailDatabase.LetterTypes
{
    public interface ILetterType
    {
        IEnumerable<int> GetLettersFromFolder(string mailbox);
    }
}
