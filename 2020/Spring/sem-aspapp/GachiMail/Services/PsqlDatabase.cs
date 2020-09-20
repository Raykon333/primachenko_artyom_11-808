using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.Json;
using System.Linq;
using MailDatabase;
using MailDatabase.Models;
using MailDatabase.LetterTypes;
using MailDatabase.Exceptions;
using GachiMail.Models;

namespace GachiMail
{
    public class PsqlDatabase : IDatabaseService
    {
        string cs;

        public PsqlDatabase(string connectionString)
        {
            cs = connectionString;
        }

        private Func<string, string> DefaultPasswordHashFunction =
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

        public bool PasswordCheck(string userLogin, string password)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
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
                        return false;
                return true;
            }
        }

        public bool DoesUserExist(string userLogin)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                return db.Users.Any(user => user.UserLogin == userLogin);
            }
        }

        public bool DoesMailboxExist(string mailboxName)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                return db.Mailboxes.Any(box => box.MailboxName == mailboxName);
            }
        }

        public void AddUser(string login, string password)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                if (DoesUserExist(login))
                    throw new DatabaseException($"User {login} already exists");
                var newUser = new User(login, DefaultPasswordHashFunction(password));
                db.Users.Add(newUser);
                db.SaveChanges();
            }
        }

        public void AddMailbox(string userLogin, string mailboxName)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
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
        private void AddMail(string mailboxName, int mailId, int folderId)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                if (!db.Mailboxes.Any(mailbox => mailbox.MailboxName == mailboxName))
                    throw new DatabaseException($"Mailbox {mailboxName} doesn't exist.");

                var newMailboxToMail = new MailboxToMails(mailboxName, mailId, folderId);
                db.MailboxesToMails.Add(newMailboxToMail);
                db.SaveChanges();
            }
        }

        public void SendMail(string title, string content,
            DateTime sendingTime, string senderMBname, params string[] receiversMBnames)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                if (receiversMBnames == null)
                    throw new DatabaseException("Can't send to no one");
                var newMail = new Mail(title, content, sendingTime, senderMBname, receiversMBnames);
                db.Mails.Add(newMail);
                db.SaveChanges();

                AddMail(senderMBname, newMail.MailId, 1);
                foreach (var receiver in receiversMBnames)
                    AddMail(receiver, newMail.MailId, 0);
                db.SaveChanges();
            }
        }

        public List<string> GetMailboxesByUser(string userLogin)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                if (!DoesUserExist(userLogin))
                    throw new DatabaseException($"User {userLogin} doesn't exist.");
                return db.UsersToMailboxes
                    .Where(r => r.UserLogin == userLogin)
                    .Select(r => r.MailboxName)
                    .ToList();
            }
        }

        public void DeleteUser(string userLogin)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                var deletedUser = db.Users.FirstOrDefault(u => u.UserLogin == userLogin);
                if (deletedUser == null)
                    throw new DatabaseException($"User {userLogin} doesn't exist.");
                db.Users.Remove(deletedUser);
                db.SaveChanges();
            }
        }

        public void DeleteMailbox(string mailboxName)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
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

        public void DeleteMail(int mailId)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
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

        public void MoveMailToFolder(string mailboxName, int mailId, int folderId)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
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

        public IEnumerable<int> GetMailIdsFromFolder(string mailboxName, int folderId)
        {
            if (!DoesMailboxExist(mailboxName))
                throw new DatabaseException($"Mailbox {mailboxName} doesn't exist.");
            var result = new List<int>();
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                var list = db.MailboxesToMails
                    .Where(x => x.MailboxName == mailboxName && x.FolderId == folderId)
                    .Select(x => x.MailId);
                foreach (var i in list)
                    result.Add(i);
            }
            return result;
        }

        public MailPreview GetMailPreview(int mailId)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                var mail = db.Mails.FirstOrDefault(m => m.MailId == mailId);
                if (mail == null)
                    throw new DatabaseException($"Mail id {mailId} not found");
                var contentPreview = mail.Content.Substring(0, Math.Min(40, mail.Content.Length));
                if (mail.Content.Length > 40)
                    contentPreview += "...";
                return new MailPreview(
                    mail.MailId, 
                    mail.Title, 
                    contentPreview, 
                    mail.SenderMBName, 
                    JsonSerializer.Deserialize<string[]>(mail.ReceiversMBNames),
                    mail.SendingTime);
            }
        }

        public string GetMailContent(int mailId)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                var mail = db.Mails.FirstOrDefault(m => m.MailId == mailId);
                if (mail == null)
                    throw new DatabaseException($"Mail id {mailId} not found");
                return mail.Content;
            }
        }

        public string[] GetMailReceivers(int mailId)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                var mail = db.Mails.FirstOrDefault(m => m.MailId == mailId);
                if (mail == null)
                    throw new DatabaseException($"Mail id {mailId} not found");
                return JsonSerializer.Deserialize<string[]>(mail.ReceiversMBNames);
            }
        }

        public IEnumerable<int> GetFoldersInMailbox(string mailboxName)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                if (!DoesMailboxExist(mailboxName))
                    throw new DatabaseException($"Mailbox {mailboxName} doesn't exist.");
                return db.FolderIdsToNames
                    .Where(x => x.MailboxName == mailboxName)
                    .Select(x => x.FolderId);
            }
        }

        public string GetFolderName(string mailboxName, int folderId)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
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

        public void AddCurrency(string userLogin, int delta)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                var user = db.Users.FirstOrDefault(u => u.UserLogin == userLogin);
                if (user == null)
                    throw new DatabaseException($"User {userLogin} doesn't exist");
                user.CurrencyAmount += delta;
                db.SaveChanges();
            }
        }

        public int GetCurrency(string userLogin)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                var user = db.Users.FirstOrDefault(u => u.UserLogin == userLogin);
                if (user == null)
                    throw new DatabaseException($"User {userLogin} doesn't exist");
                return user.CurrencyAmount;
            }
        }

        public byte GetTier(string userLogin)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                var user = db.Users.FirstOrDefault(u => u.UserLogin == userLogin);
                if (user == null)
                    throw new DatabaseException($"User {userLogin} doesn't exist");
                return user.Tier;
            }
        }

        public DateTime GetTierEndDate(string userLogin)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                var user = db.Users.FirstOrDefault(u => u.UserLogin == userLogin);
                if (user == null)
                    throw new DatabaseException($"User {userLogin} doesn't exist");
                return user.TierEndDate;
            }
        }

        public TimeSpan GetTrashTimer(string mailboxName)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                var mailbox = db.Mailboxes.FirstOrDefault(b => b.MailboxName == mailboxName);
                if (mailbox == null)
                    throw new DatabaseException($"Mailbox {mailboxName} doesn't exist.");
                return mailbox.TrashTimer;
            }
        }

        public void SetTier(string userLogin, byte tier, DateTime endDate)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                var user = db.Users.FirstOrDefault(u => u.UserLogin == userLogin);
                if (user == null)
                    throw new DatabaseException($"User {userLogin} doesn't exist");
                user.Tier = tier;
                user.TierEndDate = endDate;
                db.SaveChanges();
            }
        }

        public void UpdateTier(string userLogin)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                var user = db.Users.FirstOrDefault(u => u.UserLogin == userLogin);
                if (user == null)
                    throw new DatabaseException($"User {userLogin} doesn't exist");
                if (DateTime.Now > user.TierEndDate)
                    user.Tier = 0;
                db.SaveChanges();
            }
        }

        public void UpdateTierForAll()
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                foreach (var user in db.Users)
                    if (DateTime.Now > user.TierEndDate)
                        user.Tier = 0;
                db.SaveChanges();
            }
        }

        public void SetTrashTimer(string mailboxName, TimeSpan timer)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                var mailbox = db.Mailboxes.FirstOrDefault(b => b.MailboxName == mailboxName);
                if (mailbox == null)
                    throw new DatabaseException($"Mailbox {mailboxName} doesn't exist.");
                mailbox.TrashTimer = timer;
                db.SaveChanges();
            }
        }

        public void DeleteTimedOutTrash(string mailboxName)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
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

        public void DeleteAllTimedOutTrash()
        {
            using (DatabaseContext db = new DatabaseContext(cs))
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

        public bool MailboxContains(string mailboxName, int mailId)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                return db.MailboxesToMails.Any(r => r.MailboxName == mailboxName && r.MailId == mailId);
            }
        }

        public void ChangeRole(string userLogin, string newRole)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                var user = db.Users.FirstOrDefault(u => u.UserLogin == userLogin);
                if (user == null)
                    throw new DatabaseException($"User {userLogin} doesn't exist.");
                user.Role = newRole;
                db.SaveChanges();
            }
        }

        public string GetRole(string userLogin)
        {
            using (DatabaseContext db = new DatabaseContext(cs))
            {
                var user = db.Users.FirstOrDefault(u => u.UserLogin == userLogin);
                if (user == null)
                    throw new DatabaseException($"User {userLogin} doesn't exist.");
                return db.Users.First(u => u.UserLogin == userLogin).Role;
            }
        }
    }
}
