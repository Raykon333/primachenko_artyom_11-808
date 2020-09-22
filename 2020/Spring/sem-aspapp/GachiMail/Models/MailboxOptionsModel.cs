using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GachiMail.Models
{
    public class MailboxOptionsModel
    {
        public int folder { get; set; }
        public int pageSize { get; set; }
        public string address { get; set; }

        public MailboxOptionsModel() { }

        public MailboxOptionsModel(int _folder, int _pageSize, string _address)
        {
            folder = _folder;
            pageSize = _pageSize;
            address = _address;
        }
    }
}
