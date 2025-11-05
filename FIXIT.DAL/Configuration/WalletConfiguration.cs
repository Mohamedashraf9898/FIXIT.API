using System.Reflection.Emit;
using FIXIT.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FIXIT.DAL.Configuration
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.HasKey(w => w.Id);

            builder.HasOne(w => w.CraftsMan)
                   .WithOne(c => c.Wallet)
                   .HasForeignKey<Wallet>(w => w.CraftsManId)
                   .OnDelete(DeleteBehavior.Cascade);

          
            builder.Property(w => w.Balance)
                   .HasDefaultValue(0)
                   .HasColumnType("decimal(10,2)");

            builder.Property(w => w.LastUpdatedAt)
                   .HasDefaultValueSql("GETDATE()");

        }
    }
}
