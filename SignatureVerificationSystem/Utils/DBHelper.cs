using Microsoft.EntityFrameworkCore;
using SignatureVerificationSystem.Models.Authorization;
using System;
using System.Threading.Tasks;

namespace SignatureVerificationSystem.Utils
{
    public sealed class DBHelper
    {
        public static UserDBContext Instance;

        public static async Task InitDb()
        {
            /*
             * For AppDB run:
             * Add-Migration AppDbInitialCreate -Context AppDbContext -OutputDir "Migrations/AppDb"
             * Update-Database -Context AppDbContext
             */
            Instance = new UserDBContext();

            var user = await Instance.Users.FirstOrDefaultAsync();

            if (user == null)
            {
                var admin = new User
                {
                    Password = "b609da0d98ca3d382c0b62d1976a1dde7cfcd100d7c130699ec1c88e25e30544",
                    Username = "admin",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                Instance.Users.Add(admin);
                await Instance.SaveChangesAsync();
            }
        }
    }
}