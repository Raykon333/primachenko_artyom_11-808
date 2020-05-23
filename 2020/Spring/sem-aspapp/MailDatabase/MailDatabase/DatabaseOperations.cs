using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.Json;
using System.Linq;

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

        public static bool PasswordCheck(string login, string password)
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

        public static bool DoesUserExist(string userLogin)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                return db.Users.Any(user => user.Login == userLogin);
            }
        }

        public static bool DoesMailboxExist(string mailboxName)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                return db.Mailboxes.Any(box => box.MailboxName == mailboxName);
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

        //Приватный метод, используемый в методе SendMail
        private static void AddMail(string mailboxName, int mailId, int folderId)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                if (!db.Mailboxes.Any(mailbox => mailbox.MailboxName == mailboxName))
                    throw new ArgumentException($"Mailbox {mailboxName} doesn't exist.");

                var newMailboxToMail = new MailboxToMails(mailboxName, mailId, folderId);
                db.MailboxesToMails.Add(newMailboxToMail);
                db.SaveChanges();
            }
        }

        public static void SendMail(string title, string content, 
            DateTime sendingTime, string senderMBname, params string[] receiversMBnames)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                if (receiversMBnames == null)
                    throw new ArgumentException("Can't send to no one");
                var newMail = new Mail(title, content, sendingTime, senderMBname, receiversMBnames);
                db.Mails.Add(newMail);
                db.SaveChanges();

                AddMail(senderMBname, newMail.MailId, 1);
                foreach(var receiver in receiversMBnames)
                    AddMail(receiver, newMail.MailId, 0);
                db.SaveChanges();
            }
        }

        public static List<string> GetMailboxesByUser(string userLogin)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                if (!DoesUserExist(userLogin))
                    throw new ArgumentException();
                return db.UsersToMailboxes
                    .Where(r => r.UserLogin == userLogin)
                    .Select(r => r.MailboxName)
                    .ToList();
            }
        }

        public static void DeleteUser(string login)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var deletedUser = db.Users.FirstOrDefault(u => u.Login == login);
                if (deletedUser == null)
                    throw new ArgumentException();
                db.Users.Remove(deletedUser);
                db.SaveChanges();
            }
        }

        public static void DeleteMailbox(string mailboxName)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var deletedMailbox = db.Mailboxes.FirstOrDefault(b => b.MailboxName == mailboxName);
                if (deletedMailbox == null)
                    throw new ArgumentException();
                var deletedRship = db.UsersToMailboxes.First(r => r.MailboxName == mailboxName);
                db.Mailboxes.Remove(deletedMailbox);
                db.UsersToMailboxes.Remove(deletedRship);
                db.SaveChanges();
            }
        }

        public static void DeleteMail(int mailId)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var deletedMail = db.Mails.FirstOrDefault(m => m.MailId == mailId);
                if (deletedMail == null)
                    throw new ArgumentException();
                db.Mails.Remove(deletedMail);
                foreach (var rship in db.MailboxesToMails.Where(x => x.MailId == mailId))
                    db.MailboxesToMails.Remove(rship);
                db.SaveChanges();
            }
        }

        public static void MoveMailToFolder(string mailboxName, int mailId, int folderId)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var movedMailRship = db.MailboxesToMails
                    .FirstOrDefault(m => m.MailboxName == mailboxName && m.MailId == mailId);
                movedMailRship.FolderId = folderId;
                db.SaveChanges();
            }
        }

        public static IEnumerable<int> GetMailIdsFromFolder(string mailboxName, int folderId)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                if (!DoesMailboxExist(mailboxName))
                    throw new ArgumentException();
                return db.MailboxesToMails
                    .Where(x => x.MailboxName == mailboxName && x.FolderId == folderId)
                    .Select(x => x.MailId);
            }
        }

        public static (string Title, string ContentPreview, string Sender, DateTime SendingTime) GetMailPreview(int mailId)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var mail = db.Mails.FirstOrDefault(m => m.MailId == mailId);
                if (mail == null)
                    throw new ArgumentException();
                return (mail.Title, mail.Content.Substring(0, 40), mail.SenderMBName, mail.SendingTime);
            }
        }

        public static string GetMailContent(int mailId)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var mail = db.Mails.FirstOrDefault(m => m.MailId == mailId);
                if (mail == null)
                    throw new ArgumentException();
                return mail.Content;
            }
        }

        public static string[] GetMailReceivers(int mailId)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var mail = db.Mails.FirstOrDefault(m => m.MailId == mailId);
                if (mail == null)
                    throw new ArgumentException();
                return JsonSerializer.Deserialize<string[]>(mail.ReceiversMBNames);
            }
        }

        public static IEnumerable<int> GetFoldersInMailbox(string mailboxName)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                if (!DoesMailboxExist(mailboxName))
                    throw new ArgumentException();
                return db.FolderIdsToNames
                    .Where(x => x.MailboxName == mailboxName)
                    .Select(x => x.FolderId);
            }
        }

        public static string GetFolderName(string mailboxName, int folderId)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                if (!DoesMailboxExist(mailboxName))
                    throw new ArgumentException("Mailbox doesn't exist");
                var rship = db.FolderIdsToNames
                    .FirstOrDefault(x => x.MailboxName == mailboxName && x.FolderId == folderId);
                if (rship == null)
                    throw new ArgumentException("Folder doesn't exist");
                return rship.FolderName;
            }
        }
    }
}
