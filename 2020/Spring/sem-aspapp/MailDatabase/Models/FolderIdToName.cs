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

        [Column(Order = 3)]
        public bool NeedsUpdating { get; set; }

        public FolderIdToName() { }

        public FolderIdToName(string mailboxName, string folderName, int folderId)
        {
            MailboxName = mailboxName;
            FolderName = folderName;
            FolderId = folderId;
            NeedsUpdating = true;
        }
    }
}
