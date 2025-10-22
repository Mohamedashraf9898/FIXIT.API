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
            builder.Property(c=>c.Describtion)
                   .IsRequired()
                   .HasColumnType("nvarchar")
                   .HasMaxLength(1000);

            builder.Property(c => c.HourlyRate)
                   .HasColumnType("money");

            builder.Property(c => c.IsVerified)
                   .HasDefaultValue(false);


        }
    }
}
