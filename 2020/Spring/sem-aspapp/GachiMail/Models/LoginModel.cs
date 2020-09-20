using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GachiMail.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Введите логин")]
        [StringLength(20, ErrorMessage = "Логин слишком длинный. Введите логин не длиннее 20 символов")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
