using System;
using System.Collections.Generic;
using System.Text;

namespace MailDatabase.Exceptions
{
    public class DatabaseException : Exception
    {
        public DatabaseException() { }

        public DatabaseException(string message)
            : base(message) { }
    }
}
