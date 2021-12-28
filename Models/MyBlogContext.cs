using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RazorEF.Models
{
    // 3. Extends IdentityDbContext to use Tables: Roles, RoleClaims, Users, UserClaims,...
    public class MyBlogContext : IdentityDbContext<AppUser>
    {
        public MyBlogContext(DbContextOptions<MyBlogContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 5.2 Custome to remove prefix "AspNet"
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
        }

        public DbSet<Article> articles { get; set; }
    }
}