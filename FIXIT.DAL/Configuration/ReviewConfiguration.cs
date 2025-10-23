using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIXIT.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FIXIT.DAL.Configuration
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");

            builder.Property(r => r.RatingValue)
                   .IsRequired(); 

            builder.Property(r => r.Comment)
                   .HasMaxLength(500); 

            builder.HasOne(r => r.Client)
                   .WithMany(c => c.Reviews)
                   .HasForeignKey(r => r.ClientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.CraftsMan)
                   .WithMany(c => c.Reviews)
                   .HasForeignKey(r => r.CraftsManId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ServicesRequest)
                   .WithOne(sr => sr.Review)
                   .HasForeignKey<Review>(r => r.ServicesRequestId) 
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
