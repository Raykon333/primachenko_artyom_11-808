using System.ComponentModel.DataAnnotations;

namespace GachiMail.Models
{
    public class NewMailModel
    {
        [Required(ErrorMessage = "Введите тему")]
        [StringLength(40, ErrorMessage = "Слишком длинная тема. Тема должна быть не длиннее 40 символов")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Укажите получателей")]
        public string Receivers { get; set; }
        [Required(ErrorMessage = "Введите текст письма")]
        [StringLength(200, ErrorMessage = "Слишком длинное письмо. Содержимое должно быть не длиннее 200 символов")]
        public string Content { get; set; }
    }
}
