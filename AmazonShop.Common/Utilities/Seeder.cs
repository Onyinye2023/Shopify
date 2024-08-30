using AmazonShop.Domain.Models;
using AmazonShop.Infrastructure.Data;

namespace AmazonShop.Common.Utilities
{
    public class Seeder
    {
        public static void SeedRoles(AppDbContext context)
        {
            if (!context.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role
                    {
                        RoleName = "Admin"
                    },

                     new Role
                    {
                        RoleName = "User"
                    }
                };

                context.Roles.AddRange(roles);
                context.SaveChanges();
            }
        }

    }
}
