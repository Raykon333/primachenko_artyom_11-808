using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.Json;
using System.Linq;
using MailDatabase.Models;
using MailDatabase.LetterTypes;
using MailDatabase.Exceptions;

namespace MailDatabase
{
    public static class DatabaseOperations
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

        public static bool PasswordCheck(string userLogin, string password)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                if (!DoesUserExist(userLogin))
                    throw new DatabaseException("User doesn't exist");
                string savedPasswordHash = db.Users.First(user => user.UserLogin == userLogin).PasswordHash;
                byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
                byte[] hash = pbkdf2.GetBytes(20);
                for (int i = 0; i < 20; i++)
                    if (hashBytes[i + 16] != hash[i])
                        throw new DatabaseException();
                return true;
            }
        }

        public static bool DoesUserExist(string userLogin)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                return db.Users.Any(user => user.UserLogin == userLogin);
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
                    throw new DatabaseException($"User {login} already exists");
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
                    throw new DatabaseException($"Name {mailboxName} is already taken.");
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
                    throw new DatabaseException($"Mailbox {mailboxName} doesn't exist.");

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
                    throw new DatabaseException("Can't send to no one");
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
                    throw new DatabaseException($"User {userLogin} doesn't exist.");
                return db.UsersToMailboxes
                    .Where(r => r.UserLogin == userLogin)
                    .Select(r => r.MailboxName)
                    .ToList();
            }
        }

        public static void DeleteUser(string userLogin)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var deletedUser = db.Users.FirstOrDefault(u => u.UserLogin == userLogin);
                if (deletedUser == null)
                    throw new DatabaseException($"User {userLogin} doesn't exist.");
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
                    throw new DatabaseException($"Mailbox {mailboxName} doesn't exist.");
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
                    throw new DatabaseException($"Mail id {mailId} not found");
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
                if (!DoesMailboxExist(mailboxName))
                    throw new DatabaseException($"Mailbox {mailboxName} doesn't exist.");
                var movedMailRship = db.MailboxesToMails
                    .FirstOrDefault(m => m.MailboxName == mailboxName && m.MailId == mailId);
                if (movedMailRship == null)
                    throw new DatabaseException($"Mail id {mailId} not found");
                movedMailRship.FolderId = folderId;
                db.SaveChanges();
            }
        }

        public static IEnumerable<int> GetMailIdsFromFolder<T>(string mailboxName)
            where T: ILetterType, new()
        {
            if (!DoesMailboxExist(mailboxName))
                throw new DatabaseException($"Mailbox {mailboxName} doesn't exist.");
            return new T().GetLettersFromFolder(mailboxName);
        }

        public static (string Title, string ContentPreview, string Sender, DateTime SendingTime) GetMailPreview(int mailId)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var mail = db.Mails.FirstOrDefault(m => m.MailId == mailId);
                if (mail == null)
                    throw new DatabaseException($"Mail id {mailId} not found");
                return (mail.Title, mail.Content.Substring(0, Math.Min(40, mail.Content.Length)), mail.SenderMBName, mail.SendingTime);
            }
        }

        public static string GetMailContent(int mailId)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var mail = db.Mails.FirstOrDefault(m => m.MailId == mailId);
                if (mail == null)
                    throw new DatabaseException($"Mail id {mailId} not found");
                return mail.Content;
            }
        }

        public static string[] GetMailReceivers(int mailId)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var mail = db.Mails.FirstOrDefault(m => m.MailId == mailId);
                if (mail == null)
                    throw new DatabaseException($"Mail id {mailId} not found");
                return JsonSerializer.Deserialize<string[]>(mail.ReceiversMBNames);
            }
        }

        public static IEnumerable<int> GetFoldersInMailbox(string mailboxName)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                if (!DoesMailboxExist(mailboxName))
                    throw new DatabaseException($"Mailbox {mailboxName} doesn't exist.");
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
                    throw new DatabaseException($"Mailbox {mailboxName} doesn't exist.");
                var rship = db.FolderIdsToNames
                    .FirstOrDefault(x => x.MailboxName == mailboxName && x.FolderId == folderId);
                if (rship == null)
                    throw new DatabaseException("Folder doesn't exist");
                return rship.FolderName;
            }
        }

        public static void AddCurrency(string userLogin, int delta)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var user = db.Users.FirstOrDefault(u => u.UserLogin == userLogin);
                if (user == null)
                    throw new DatabaseException($"User {userLogin} doesn't exist");
                user.CurrencyAmount += delta;
                db.SaveChanges();
            }
        }

        public static int GetCurrency(string userLogin)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var user = db.Users.FirstOrDefault(u => u.UserLogin == userLogin);
                if (user == null)
                    throw new DatabaseException($"User {userLogin} doesn't exist");
                return user.CurrencyAmount;
            }
        }

        public static byte GetTier(string userLogin)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var user = db.Users.FirstOrDefault(u => u.UserLogin == userLogin);
                if (user == null)
                    throw new DatabaseException($"User {userLogin} doesn't exist");
                return user.Tier;
            }
        }

        public static DateTime GetTierEndDate(string userLogin)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var user = db.Users.FirstOrDefault(u => u.UserLogin == userLogin);
                if (user == null)
                    throw new DatabaseException($"User {userLogin} doesn't exist");
                return user.TierEndDate;
            }
        }

        public static TimeSpan GetTrashTimer(string mailboxName)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var mailbox = db.Mailboxes.FirstOrDefault(b => b.MailboxName ==mailboxName);
                if (mailbox == null)
                    throw new DatabaseException($"Mailbox {mailboxName} doesn't exist.");
                return mailbox.TrashTimer;
            }
        }

        public static void SetTier(string userLogin, byte tier, DateTime endDate)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var user = db.Users.FirstOrDefault(u => u.UserLogin == userLogin);
                if (user == null)
                    throw new DatabaseException($"User {userLogin} doesn't exist");
                user.Tier = tier;
                user.TierEndDate = endDate;
                db.SaveChanges();
            }
        }

        public static void UpdateTier(string userLogin)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var user = db.Users.FirstOrDefault(u => u.UserLogin == userLogin);
                if (user == null)
                    throw new DatabaseException($"User {userLogin} doesn't exist");
                if (DateTime.Now > user.TierEndDate)
                    user.Tier = 0;
                db.SaveChanges();
            }
        }

        public static void UpdateTierForAll()
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                foreach (var user in db.Users)
                    if (DateTime.Now > user.TierEndDate)
                        user.Tier = 0;
                db.SaveChanges();
            }
        }

        public static void SetTrashTimer(string mailboxName, TimeSpan timer)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var mailbox = db.Mailboxes.FirstOrDefault(b => b.MailboxName == mailboxName);
                if (mailbox == null)
                    throw new DatabaseException($"Mailbox {mailboxName} doesn't exist.");
                mailbox.TrashTimer = timer;
                db.SaveChanges();
            }
        }

        public static void DeleteTimedOutTrash(string mailboxName)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                var mailbox = db.Mailboxes.FirstOrDefault(b => b.MailboxName == mailboxName);
                if (mailbox == null)
                    throw new DatabaseException($"Mailbox {mailboxName} doesn't exist.");
                foreach (var mail in db.MailboxesToMails
                    .Where(r => r.MailboxName == mailboxName && r.FolderId == 2)
                    .Select(r => db.Mails.First(m => m.MailId == r.MailId)))
                {
                    if (mail.SendingTime + mailbox.TrashTimer > DateTime.Now)
                        db.Mails.Remove(mail);
                }
                db.SaveChanges();
            }
        }

        public static void DeleteAllTimedOutTrash()
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                Dictionary<string, TimeSpan> timers = new Dictionary<string, TimeSpan>();
                foreach (var mailbox in db.Mailboxes)
                    timers.Add(mailbox.MailboxName, mailbox.TrashTimer);
                foreach (var rship in db.MailboxesToMails
                    .Where(r => r.FolderId == 2))
                {
                    var mail = db.Mails.First(m => m.MailId == rship.MailId);
                    if (mail.SendingTime + timers[rship.MailboxName] > DateTime.Now)
                        db.Mails.Remove(mail);
                }
                db.SaveChanges();
            }
        }
    }
}
