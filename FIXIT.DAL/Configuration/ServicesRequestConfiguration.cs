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
    public class ServicesRequestConfiguration : IEntityTypeConfiguration<ServicesRequest>
    {
        public void Configure(EntityTypeBuilder<ServicesRequest> builder)
        {
            builder.ToTable("ServicesRequests");
            builder.HasKey(sr => sr.ServicesRequestId);
            builder.Property(sr => sr.ServicesRequestId)
                   .ValueGeneratedOnAdd();

            builder.Property(sr => sr.ServiceRequestImage).IsRequired(false);
            builder.Property(sr => sr.Location).IsRequired(false);
            builder.Property(sr => sr.CraftsManId)
                   .IsRequired(false);
            builder.Property(sr => sr.Status)
                   .HasConversion<string>()
                   .HasDefaultValue(ServiceRequestStatus.Pending);
            builder.Property(e => e.IsCancelled)
                   .HasDefaultValue(false);

            builder.Property(sr => sr.RequestAt)
                   .HasDefaultValueSql("GETDATE()"); 

            // علاقة رأس-ببرأس مع Payment (طلب الخدمة الواحد لديه عملية دفع واحدة)
            //builder.HasOne(sr => sr.Payment)
            //       .WithOne(p => p.ServicesRequest)
            //       .HasForeignKey<Payment>(p => p.ServicesRequestId) // المفتاح الأجنبي في جدول Payment
            //       .IsRequired(false) // الدفع قد يكون غير موجود في البداية
            //       .OnDelete(DeleteBehavior.Cascade); // يمكن حذف الدفع بحذف الطلب

            builder.HasOne(sr => sr.Review)
                   .WithOne(r => r.ServicesRequest)
                   .HasForeignKey<Review>(r => r.ServicesRequestId) 
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.WalletTransaction)
             .WithOne(t => t.ServicesRequest)
             .HasForeignKey<WalletTransaction>(t => t.ServiceRequestId)
             .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
