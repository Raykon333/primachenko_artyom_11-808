using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MailDatabase.Models
{
    class FolderIdToName
    {
        [Column(Order = 0)]
        public string MailboxName { get; internal set; }

        [Column(Order = 1)]
        public int FolderId { get; internal set; }

        [Column(Order = 2)]
        public string FolderName { get; internal set; }

        public FolderIdToName() { }

        public FolderIdToName(string mailboxName, int folderId, string folderName)
        {
            MailboxName = mailboxName;
            FolderId = folderId;
            FolderName = folderName;
        }
    }
}
