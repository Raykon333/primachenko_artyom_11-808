using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace MailDatabase
{
    internal class DatabaseContext : DbContext
    {
        readonly static string Host = "localhost";
        readonly static string Port = "5432";
        readonly static string DatabaseName = "MailDB";
        readonly static string Username = "postgres";
        readonly static string Password = "postgres";

        internal DbSet<User> Users { get; set; }
        internal DbSet<Mailbox> Mailboxes { get; set; }
        internal DbSet<Mail> Mails { get; set; }
        internal DbSet<UserToMailboxes> UsersToMailboxes { get; set; }
        internal DbSet<MailboxToMails> MailboxesToMails { get; set; }
        internal DbSet<FolderIdToName> FolderIdsToNames { get; set; }
        internal DbSet<UserTrashTimer> UsersTrashTimers { get; set; }

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
                .HasKey(o => new { o.MailboxName, o.MailId });
            modelBuilder.Entity<MailboxToMails>()
                .HasIndex(o => o.MailboxName);
            modelBuilder.Entity<MailboxToMails>()
                .HasIndex(o => new { o.MailboxName, o.FolderId });

            modelBuilder.Entity<FolderIdToName>()
                .HasKey(o => new { o.MailboxName, o.FolderId });
            modelBuilder.Entity<FolderIdToName>()
                .HasIndex(o => o.MailboxName);

            modelBuilder.Entity<UserTrashTimer>()
                .HasKey(o => new { o.UserLogin, o.TrashTimer });
            modelBuilder.Entity<UserTrashTimer>()
                .HasIndex(o => o.UserLogin);
        }

        public DatabaseContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql($"Host={Host};Port={Port};Database={DatabaseName};Username={Username};Password={Password}");
        }
    }
}
