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
    public class CraftsManServiceConfiguration : IEntityTypeConfiguration<CraftsManService>
    {
        public void Configure(EntityTypeBuilder<CraftsManService> builder)
        {
            builder.ToTable("CraftsMenServices");

            builder.HasKey(cs => new { cs.CraftsManId, cs.ServiceId });

            builder.Property(cs => cs.HourlyRate)
                   .HasColumnType("money")
                   .IsRequired();

            builder.HasOne(cs => cs.CraftsMan)
                   .WithMany(c => c.CraftsManServices)
                   .HasForeignKey(cs => cs.CraftsManId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cs => cs.Service)
                   .WithMany(s => s.CraftsManServices)
                   .HasForeignKey(cs => cs.ServiceId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
