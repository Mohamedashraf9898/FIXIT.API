
using FIXIT.BLL.Interfaces;
using FIXIT.BLL.Repositories;
using FIXIT.BLL.Services;
using FIXIT.DAL;
using FIXIT.DAL.Models;
using Microsoft.EntityFrameworkCore;
using CraftsManService = FIXIT.BLL.Services.CraftsManService;

namespace FIXIT.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<FixItDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("FixItConnectionString"));
            });
            builder.Services.AddScoped<IGenericRepository<CraftsMan>, CraftsManRepo>(); 
            builder.Services.AddScoped<ICraftsManService, CraftsManService>();

			var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
