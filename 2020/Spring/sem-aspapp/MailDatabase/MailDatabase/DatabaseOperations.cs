using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;

namespace MailDatabase
{
    static class DatabaseOperations
    {
        private static Func<string, string> DefaultPasswordHashFunction =
            password =>
            {
                byte[] salt;
                new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
                byte[] hash = pbkdf2.GetBytes(20);
                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);
                return Convert.ToBase64String(hashBytes);
            };

        private static bool PasswordCheck(string login, string password)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                if (!DoesUserExist(login))
                    throw new ArgumentException($"User {login} doesn't exist");
                string savedPasswordHash = db.Users.First(user => user.Login == login).PasswordHash;
                byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
                byte[] hash = pbkdf2.GetBytes(20);
                for (int i = 0; i < 20; i++)
                    if (hashBytes[i + 16] != hash[i])
                        throw new UnauthorizedAccessException();
                return true;
            }
        }

        private static bool DoesUserExist(string userLogin)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                return db.Users.Any(user => user.Login == userLogin);
            }
        }

        public static void AddUser(string login, string password)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                if (DoesUserExist(login))
                    throw new ArgumentException($"User {login} already exists");
                var newUser = new User(login, DefaultPasswordHashFunction(password));
                db.Users.Add(newUser);
                db.SaveChanges();
            }
        }

        public static void AddMailbox(string userLogin, string mailboxName)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                if (db.Mailboxes.Any(mailbox => mailbox.MailboxName == mailboxName))
                    throw new ArgumentException($"Name {mailboxName} is already taken.");
                var newMailbox = new Mailbox(mailboxName);
                var newUserToMailbox = new UserToMailboxes(userLogin, mailboxName);
                db.Mailboxes.Add(newMailbox);
                db.UsersToMailboxes.Add(newUserToMailbox);
                db.SaveChanges();
            }
        }

        public static void AddMail(string mailboxName, string title, string content)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                if (!db.Mailboxes.Any(mailbox => mailbox.MailboxName == mailboxName))
                    throw new ArgumentException($"Mailbox {mailboxName} doesn't exist.");
                int newMailId = db.Mails.Count();
                var newMail = new Mail(newMailId, title, content);
                var newMailboxToMail = new MailboxToMails(mailboxName, newMailId);
                db.Mails.Add(newMail);
                db.MailboxesToMails.Add(newMailboxToMail);
                db.SaveChanges();
            }
        }
    }
}
