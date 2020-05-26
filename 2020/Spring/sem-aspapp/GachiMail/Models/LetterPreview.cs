using System;
using MailDatabase;

namespace GachiMail.Models
{
    public class LetterPreview
    {
        public string Title { get; private set; }
        public string ContentPreview { get; private set; }
        public string Sender { get; private set; }
        public DateTime SendingTime { get; private set; }
        public LetterPreview(int ID)
        {
            var dbresult = DatabaseOperations.GetMailPreview(ID);
            Title = dbresult.Title;
            ContentPreview = dbresult.ContentPreview;
            Sender = dbresult.Sender;
            SendingTime = dbresult.SendingTime;
        }
    }
}
