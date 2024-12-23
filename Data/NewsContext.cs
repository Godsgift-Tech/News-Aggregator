using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using News_Aggregator.EntityModel;
using News_Aggregator.Models.DomainModel;


namespace News_Aggregator.Data
{

    public class NewsContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public NewsContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<SavedHistory> SearchHistories { get; set; }
        public DbSet<SavedArticle> SavedArticles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var hasher = new PasswordHasher<ApplicationUser>();
            base.OnModelCreating(modelBuilder); // calling base method for Identity configuration

            // Configure SearchHistory
            modelBuilder.Entity<SavedHistory>()
                .HasKey(sh => sh.SearchHistoryId);
            modelBuilder.Entity<SavedHistory>()
                .HasOne(sh => sh.User)
                .WithMany(u => u.SearchHistories)
                .HasForeignKey(sh => sh.UserId);

            // Configure SavedArticle
            modelBuilder.Entity<SavedArticle>()
                .HasKey(sa => sa.SavedArticleId);
            modelBuilder.Entity<SavedArticle>()
                .HasOne(sa => sa.User)
                .WithMany(u => u.SavedArticles)
                .HasForeignKey(sa => sa.UserId);

            // Seed roles (User, Admin, SuperAdmin)
            var adminRoleId = Guid.NewGuid();
            var superAdminRoleId = Guid.NewGuid();
            var userRoleId = Guid.NewGuid();

            var roles = new List<IdentityRole<Guid>>
        {
            new IdentityRole<Guid>
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = adminRoleId.ToString()
            },
            new IdentityRole<Guid>
            {
                Id = superAdminRoleId,
                Name = "SuperAdmin",
                NormalizedName = "SUPERADMIN",
                ConcurrencyStamp = superAdminRoleId.ToString()
            },
            new IdentityRole<Guid>
            {
                Id = userRoleId,
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = userRoleId.ToString()
            }
        };

            modelBuilder.Entity<IdentityRole<Guid>>().HasData(roles);

            // Seed SuperAdmin user
            var superAdminId = Guid.NewGuid();
            var superAdminUser = new ApplicationUser
            {
                Id = superAdminId,
                UserName = "TrendNewsSuperAdmin",
                NormalizedUserName = "TRENDNEWRSUPERADMIN",
                Email = "superadmin@newsaggregatortoday.com",
                NormalizedEmail = "SUPERADMIN@NEWSAGGREGATORTODAY.COM",
                EmailConfirmed = true
            };

            // Creating password for the SuperAdmin
            superAdminUser.PasswordHash = hasher.HashPassword(superAdminUser, "superAdminId@4Q");

            modelBuilder.Entity<ApplicationUser>().HasData(superAdminUser);

            // Add all roles to SuperAdmin user
            var superAdminRoles = new List<IdentityUserRole<Guid>>
        {
            new IdentityUserRole<Guid>
            {
                RoleId = adminRoleId,
                UserId = superAdminId
            },
            new IdentityUserRole<Guid>
            {
                RoleId = superAdminRoleId,
                UserId = superAdminId
            },
            new IdentityUserRole<Guid>
            {
                RoleId = userRoleId,
                UserId = superAdminId
            }
        };

            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(superAdminRoles);
        }
    }
}

