using FIXIT.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.DAL.Configuration
{
    public class CraftsManConfiguration : IEntityTypeConfiguration<CraftsMan>
    {
        public void Configure(EntityTypeBuilder<CraftsMan> builder)
        {
            builder.ToTable("CraftsMen");

            builder.Property(c => c.Describtion)
                   .IsRequired()
                   .HasColumnType("nvarchar")
                   .HasMaxLength(1000);

            builder.Property(c => c.HourlyRate)
                   .HasColumnType("money");

            builder.Property(c => c.IsVerified)
                   .HasDefaultValue(false);

            builder.Property(c => c.ProfileImage)
                   .HasMaxLength(255);
            builder.Property(c => c.ExperienceOfYears)
                   .IsRequired(); 

            builder.HasMany(c => c.ServicesRequests)
                   .WithOne(sr => sr.CraftsMan)
                   .HasForeignKey(sr => sr.CraftsManId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Reviews)
                   .WithOne(r => r.CraftsMan)
                   .HasForeignKey(r => r.CraftsManId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.CraftsManServices)
                   .WithOne(cs => cs.CraftsMan)
                   .HasForeignKey(cs => cs.CraftsManId);


        }
    }
}
