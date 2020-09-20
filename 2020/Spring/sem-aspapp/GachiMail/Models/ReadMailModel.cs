using System;

namespace GachiMail.Models
{
    public class ReadMailModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime SendingTime { get; set; }
        public string[] Addresses { get; set; }

        public ReadMailModel(string title, string content, DateTime sendTime, string[] adresses)
        {
            Title = title;
            Content = content;
            SendingTime = sendTime;
            Addresses = adresses;
        }

        public ReadMailModel() { }
    }
}
