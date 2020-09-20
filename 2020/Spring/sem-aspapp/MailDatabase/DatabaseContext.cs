using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using MailDatabase.Models;
namespace MailDatabase
{
    public class DatabaseContext : DbContext
    {
        string ConnectionString;

        public DbSet<User> Users { get; set; }
        public DbSet<Mailbox> Mailboxes { get; set; }
        public DbSet<Mail> Mails { get; set; }
        public DbSet<UserToMailboxes> UsersToMailboxes { get; set; }
        public DbSet<MailboxToMails> MailboxesToMails { get; set; }
        public DbSet<FolderIdToName> FolderIdsToNames { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(o => o.UserLogin);
            modelBuilder.Entity<User>()
                .HasIndex(o => o.UserLogin)
                .IsUnique();

            modelBuilder.Entity<Mailbox>()
                .HasKey(o => o.MailboxName);
            modelBuilder.Entity<Mailbox>()
                .HasIndex(o => o.MailboxName)
                .IsUnique();

            modelBuilder.Entity<Mail>()
                .HasKey(o => o.MailId);
            modelBuilder.Entity<Mail>()
                .HasIndex(o => o.MailId)
                .IsUnique();

            modelBuilder.Entity<UserToMailboxes>()
                .HasKey(o => new { o.UserLogin, o.MailboxName });
            modelBuilder.Entity<UserToMailboxes>()
                .HasIndex(o => o.UserLogin);

            modelBuilder.Entity<MailboxToMails>()
                .HasKey(o => new { o.MailboxName, o.MailId, o.FolderId });
            modelBuilder.Entity<MailboxToMails>()
                .HasIndex(o => o.MailboxName);
            modelBuilder.Entity<MailboxToMails>()
                .HasIndex(o => new { o.MailboxName, o.FolderId });

            modelBuilder.Entity<FolderIdToName>()
                .HasKey(o => new { o.MailboxName, o.FolderId });
            modelBuilder.Entity<FolderIdToName>()
                .HasIndex(o => o.MailboxName);
        }

        public DatabaseContext(string connectionString)
        {
            ConnectionString = connectionString;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(ConnectionString);
        }
    }
}
