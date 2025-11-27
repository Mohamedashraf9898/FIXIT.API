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
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title)
                   .IsRequired()
                   .HasMaxLength(200); 

            builder.Property(n => n.Message)
                   .IsRequired()
                   .HasMaxLength(1000); 

            builder.Property(n => n.IsRead)
                   .HasDefaultValue(false);

            builder.Property(n => n.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()"); 

            // Relationship with ServiceRequest
            builder.HasOne(n => n.ServiceRequest)
                   .WithMany() 
                   .HasForeignKey(n => n.ServiceRequestId)
                   .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
