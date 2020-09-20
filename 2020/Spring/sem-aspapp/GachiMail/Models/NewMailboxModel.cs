using System.ComponentModel.DataAnnotations;

namespace GachiMail.Models
{
    public class NewMailboxModel
    {
        [Required(ErrorMessage = "Введите имя ящика")]
        public string MailboxName { get; set; }
    }
}
