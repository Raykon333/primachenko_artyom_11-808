using MailDatabase;
using System;

namespace GachiMail.Models
{
    public class Letter
    {
        private int MailId;
        private LetterPreview preview;
        public LetterPreview Preview
        {
            get
            {
                if(preview == null)
                    preview = new LetterPreview(MailId);
                return preview;
            }
        }
        public string Title
        {
            get
            {
                return Preview.Title;
            }
        }
        public string Content
        {
            get
            {
                return DatabaseOperations.GetMailContent(MailId);
            }
        }
        public DateTime SendTime
        {
            get
            {
                return Preview.SendingTime;
            }
        }
        public string Sender
        {
            get
            {
                return Preview.Sender;
            }
        }
        public string[] Receivers
        {
            get
            {
                return DatabaseOperations.GetMailReceivers(MailId);
            }
        }
        public Letter(int id)
        {
            MailId = id;
        }
    }
}
