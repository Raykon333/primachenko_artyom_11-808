using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace MailDatabase.Models
{
    public class Mail
    {
        [Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MailId { get; set; }

        [Column(Order = 1), MaxLength(40)]
        public string Title { get; set; }

        [Column(Order = 2), MaxLength(200)]
        public string Content { get; set; }

        [Column(Order = 3)]
        public DateTime SendingTime { get; set; }

        [Column(Order = 4), MaxLength(30)]
        public string SenderMBName { get; set; }

        [Column(Order = 5)]
        public string ReceiversMBNames { get; set; }

        public Mail() { }

        public Mail(string title, string content, DateTime sendingTime, string senderMBName, string[] receiverMBName)
        {
            Title = title;
            Content = content;
            SendingTime = sendingTime;
            SenderMBName = senderMBName;
            ReceiversMBNames = JsonSerializer.Serialize(receiverMBName);
        }
    }
}
