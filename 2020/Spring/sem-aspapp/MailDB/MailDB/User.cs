using MailWorker;

namespace MailDB
{
    public class User
    {
        public string Address;
        public string Password;
        public FileWorker Mail = new FileWorker(@"D:\Mail");
    }
}
