using GachiMail.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GachiMail
{
    public interface IDatabaseService
    {
        public bool PasswordCheck(string userLogin, string password);

        public bool DoesUserExist(string userLogin);
        public void AddUser(string login, string password);
        public string GetRole(string userLogin);
        public void ChangeRole(string userLogin, string newRole);
        public void DeleteUser(string userLogin);

        public bool DoesMailboxExist(string mailboxName);
        public bool MailboxContains(string mailboxName, int mailId);
        public void AddMailbox(string userLogin, string mailboxName);
        public void DeleteMailbox(string mailboxName);

        public void SendMail(string title, string content,
            DateTime sendingTime, string senderMBname, params string[] receiversMBnames);
        public void MoveMailToFolder(string mailboxName, int mailId, int folderId);
        public MailPreview GetMailPreview(int mailId);
        public string GetMailContent(int mailId);
        public string[] GetMailReceivers(int mailId);
        public void DeleteMail(int mailId);

        public List<string> GetMailboxesByUser(string userLogin);
        public IEnumerable<int> GetFoldersInMailbox(string mailboxName);
        public IEnumerable<int> GetMailIdsFromFolder(string mailboxName, int folderId);

        public void CreateFolder(string mailboxName, string folderName);
        public string GetFolderName(string mailboxName, int folderId);
        public bool FolderNeedsUpdating(string mailboxName, int folderId);

        public void AddCurrency(string userLogin, int delta);
        public int GetCurrency(string userLogin);

        public byte GetTier(string userLogin);
        public DateTime GetTierEndDate(string userLogin);
        public void SetTier(string userLogin, byte tier, DateTime endDate);
        public void UpdateTier(string userLogin);
        public void UpdateTierForAll();

        public TimeSpan GetTrashTimer(string mailboxName);
        public void SetTrashTimer(string mailboxName, TimeSpan timer);
        public void DeleteTimedOutTrash(string mailboxName);
        public void DeleteAllTimedOutTrash();
    }
}
