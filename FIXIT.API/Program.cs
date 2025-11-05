
using System.Threading.Tasks;
using FIXIT.BLL.Mapping;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Repositories.Repo;
using FIXIT.BLL.Services.Intrfaces;
using FIXIT.BLL.Services.IService;
using FIXIT.BLL.Services.Service;
using FIXIT.DAL;
using FIXIT.DAL.Models;
using Microsoft.EntityFrameworkCore;
using CraftsManService = FIXIT.BLL.Services.Service.CraftsManService;

namespace FIXIT.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<FixItDbContext>(options =>
            {
                options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("FixItConnectionString"));
            });



            #region injection

            builder.Services.AddAutoMapper(op => op.AddProfile<MappingProfile>()); // Mapping Registration
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            //craftsman
            builder.Services.AddScoped<ICraftsManRepo, CraftsManRepo>();
            builder.Services.AddScoped<ICraftsManService, CraftsManService>();
            //client
            builder.Services.AddScoped<IClientService, ClientService>();
            //SertviceRequest
            builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
            builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
            //review
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            //wallet
            builder.Services.AddScoped<IWalletService, WalletService>();
            builder.Services.AddScoped<IWalletRepository, WalletRepository>();
            builder.Services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
            #endregion

            var app = builder.Build();

            var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<FixItDbContext>();
            var logger = services.GetRequiredService<ILogger<Program>>();
            try
            {
                await context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database.");
            }

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
