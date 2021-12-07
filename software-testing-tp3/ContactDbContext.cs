using Microsoft.EntityFrameworkCore;

namespace software_testing_tp3
{
    public class ContactDbContext : DbContext
    {
 
        public DbSet<Contact> Contacts { get; set; }

        private int DbSize;

        public ContactDbContext(int dbSize)
        {
            DbSize = dbSize;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($@"Data Source=contacts-{DbSize}.db;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>().HasKey(contact => contact.Id);
            modelBuilder.Entity<Contact>().Property(contact => contact.Email);
            modelBuilder.Entity<Contact>().Property(contact => contact.Name);
        }
    }
}