using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GachiMail.Models
{
    public class MailboxOptionsModel
    {
        public int folder { get; set; }
        public string sorting { get; set; }
        public int pageSize { get; set; }
        public string address { get; set; }
    }
}
