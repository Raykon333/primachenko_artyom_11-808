using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingHW
{
    public class AccountData
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public AccountData(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
