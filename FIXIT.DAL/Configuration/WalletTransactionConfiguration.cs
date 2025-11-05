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
    public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
          
            builder.HasKey(t => t.Id);

            
            builder.HasOne(t => t.Wallet)
                   .WithMany(w => w.WalletTransactions)
                   .HasForeignKey(t => t.WalletId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(t => t.ServicesRequest)
                .WithOne(s => s.WalletTransaction)
                .HasForeignKey<WalletTransaction>(t => t.ServiceRequestId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Property(t => t.Amount)
                   .HasColumnType("decimal(10,2)");

           
            //builder.Property(t => t.TransactionType)
            //       .IsRequired()
            //       .HasMaxLength(50);

            //builder.Property(t => t.Description)
            //       .HasMaxLength(200);

            
            builder.Property(t => t.CreatedAt)
                   .HasDefaultValueSql("GETDATE()");
        }
    }
}
