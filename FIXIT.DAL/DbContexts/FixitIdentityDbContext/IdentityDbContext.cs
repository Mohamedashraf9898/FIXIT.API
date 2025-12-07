using FIXIT.DAL.Models;
using FIXIT.DAL.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FIXIT.DAL.DbContexts.FixitIdentityDbContext
{
    public class IdentityDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
               : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new Configuration.Identity.ApplicationUserConfiguration());
           
        }

        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    }
}
