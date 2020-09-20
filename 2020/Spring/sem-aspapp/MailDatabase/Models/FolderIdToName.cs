using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MailDatabase.Models
{
    public class FolderIdToName
    {
        [Column(Order = 0)]
        public string MailboxName { get; set; }

        [Column(Order = 1)]
        public int FolderId { get; set; }

        [Column(Order = 2)]
        public string FolderName { get; set; }

        public FolderIdToName() { }

        public FolderIdToName(string mailboxName, int folderId, string folderName)
        {
            MailboxName = mailboxName;
            FolderId = folderId;
            FolderName = folderName;
        }
    }
}
