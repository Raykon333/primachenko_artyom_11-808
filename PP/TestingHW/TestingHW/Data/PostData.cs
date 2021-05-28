using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingHW
{
    public class PostData
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public PostData(string username, string password)
        {
            Title = username;
            Content = password;
        }

        public PostData() { }
    }
}
