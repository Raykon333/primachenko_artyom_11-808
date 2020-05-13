using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MailDatabase
{
    class Mail
    {
        [Column(Order = 0)]
        public int MailId { get; internal set; }

        [Column(Order = 1), MaxLength(40)]
        public string Title { get; internal set; }

        [Column(Order = 2), MaxLength(200)]
        public string Content { get; internal set; }

        public Mail() { }

        public Mail(int mailId, string title, string content)
        {
            MailId = mailId;
            Title = title;
            Content = content;
        }
    }
}
