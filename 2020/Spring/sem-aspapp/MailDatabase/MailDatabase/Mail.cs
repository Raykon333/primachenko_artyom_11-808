using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailDatabase
{
    class Mail
    {
        [Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MailId { get; internal set; }

        [Column(Order = 1), MaxLength(40)]
        public string Title { get; internal set; }

        [Column(Order = 2), MaxLength(200)]
        public string Content { get; internal set; }

        public Mail() { }

        public Mail(string title, string content)
        {
            Title = title;
            Content = content;
        }
    }
}
