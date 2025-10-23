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
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.Property(s => s.ServiceName)
                .IsRequired()
                .HasColumnType("nvarchar")
                .HasMaxLength(100);

            builder.Property(s => s.InitialPrice)
                .HasColumnType("money")
                .IsRequired();

            builder.HasMany(s => s.ServicesRequests) 
                   .WithOne(sr => sr.Service)
                   .HasForeignKey(sr => sr.ServiceId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.CraftsManServices)
                   .WithOne(cs => cs.Service)
                   .HasForeignKey(cs => cs.ServiceId);
        }
    }
}
