using Microsoft.EntityFrameworkCore;

namespace SignatureVerificationSystem.Models.Authorization
{
    public class UserDBContext : DbContext
    {
        private readonly string conString =
            "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog = master; Integrated Security = True; Connect Timeout = 30; Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(conString);
        }
    }
}