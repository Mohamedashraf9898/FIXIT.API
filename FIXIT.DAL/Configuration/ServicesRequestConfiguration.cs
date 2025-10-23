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

           
            builder.Property(sr => sr.Status)
                   .HasDefaultValue(ServiceRequestStatus.Pending);

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

        }
    }
}
