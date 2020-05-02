using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace MailDB
{
    public class MailContext : DbContext
    {
        public DbSet<User> UsersList;
        public MailContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasKey("Address");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseNpgsql
                ("Host=localhost;" +
                "Port=5432;" +
                "Database=MailDB;" +
                "Username=postgres;" +
                "Password=ja2min31");
        }
    }
}
