using System;
using MailDatabase;

namespace GachiMail.Models
{
    public class MailPreview
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string ContentPreview { get; private set; }
        public string Sender { get; private set; }
        public string[] Receivers { get; private set; }
        public DateTime SendingTime { get; private set; }
        public MailPreview(int ID, string title, string preview, string sender, string[] receivers, DateTime sendingTime)
        {
            Id = ID;
            Title = title;
            ContentPreview = preview;
            Sender = sender;
            Receivers = receivers;
            SendingTime = sendingTime;
        }

        public MailPreview() { }
    }
}
