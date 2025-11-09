using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIXIT.DAL.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FIXIT.DAL.Configuration.Identity
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.FName)
                .HasMaxLength(150)
                .IsRequired(false);

            builder.Property(u => u.LName)
                .HasMaxLength(150)
                .IsRequired(false);

            builder.Property(u => u.Location)
                .HasMaxLength(200)
                .IsRequired(false);

            
        }
    }
}
