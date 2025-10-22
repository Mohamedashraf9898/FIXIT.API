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
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("Clients");

            builder.HasMany(c => c.ServicesRequest)  
                   .WithOne(sr => sr.Client) 
                   .HasForeignKey(sr => sr.ClientId) 
                   .IsRequired() 
                   .OnDelete(DeleteBehavior.Restrict); 

          
            builder.HasMany(c => c.Reviews)
                   .WithOne(r => r.Client)
                   .HasForeignKey(r => r.ClientId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
