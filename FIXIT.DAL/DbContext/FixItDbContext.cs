using FIXIT.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
        
namespace FIXIT.DAL
{
    public class FixItDbContext : DbContext
    {

        public FixItDbContext(DbContextOptions<FixItDbContext> option) : base(option)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }
        

        public DbSet<Client> Clients { get; set; }
        public DbSet<CraftsMan> CraftsMan { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServicesRequest> ServicesRequests { get; set; }

    }
}
