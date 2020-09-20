using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GachiMail.Models
{
    public class AddCurrencyModel
    {
        [Required(ErrorMessage = "Введите сумму")]
        [Range(1, 1000, ErrorMessage = "Введите число от 1 до 1000")]
        public int Currency { get; set; }

        public AddCurrencyModel(int currency)
        {
            Currency = currency;
        }

        public AddCurrencyModel() { }
    }
}
