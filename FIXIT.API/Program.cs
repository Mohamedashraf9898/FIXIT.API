using System.Text;

using System.Threading.Tasks;
using FIXIT.BLL;
using FIXIT.API.Erorrs;
using FIXIT.API.Midelwaers;
using FIXIT.BLL.Mapping;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Repositories.Repo;
using FIXIT.BLL.Services;
using FIXIT.BLL.Services.Intrfaces;
using FIXIT.BLL.Services.IService;
using FIXIT.BLL.Services.IService.IAuth;
using FIXIT.BLL.Services.IService.Payment;
using FIXIT.BLL.Services.Service;
using FIXIT.BLL.Services.Service.Auth;
using FIXIT.BLL.Services.Service.Payment;
using FIXIT.DAL;
using FIXIT.DAL.DbContexts.FixitIdentityDbContext;
using FIXIT.DAL.Models.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CraftsManService = FIXIT.BLL.Services.Service.CraftsManService;

namespace FIXIT.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = false;
                options.InvalidModelStateResponseFactory = (context) =>
                {
                    var errors = context.ModelState
                    .Where(e => e.Value!.Errors.Count > 0)
                    .Select(P => new ApiValidationErorrResponse.ValditonErorr()
                    {
                        Field = P.Key,
                        Erorrs = P.Value!.Errors.Select(E => E.ErrorMessage)

                    });

                    return new BadRequestObjectResult(new ApiValidationErorrResponse()
                    {
                        Errors = errors

                    });
                };

            }); 
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<FixItDbContext>(options =>
            {
                options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("FixItConnectionString"));
            });

            builder.Services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnectionString"));
            });


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FixItPolicy",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            #region injection

            builder.Services.AddAutoMapper(op => op.AddProfile<MappingProfile>()); // Mapping Registration
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            //craftsman
            builder.Services.AddScoped<ICraftsManRepo, CraftsManRepo>();
            builder.Services.AddScoped<ICraftsManService, CraftsManService>();
            //client
            builder.Services.AddScoped<IClientRepo, ClientRepo>();
            builder.Services.AddScoped<IClientService, ClientService>();
            //SertviceRequest
            builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
            builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
            //offer
            builder.Services.AddScoped<IOfferRepository, OfferRepository>();
            builder.Services.AddScoped<IOfferService, OfferService>();

            //review
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            //service
            builder.Services.AddScoped<IServiceService, ServiceServices>();


            //wallet
            builder.Services.AddScoped<IWalletService, WalletService>();
            builder.Services.AddScoped<IWalletRepository, WalletRepository>();
            builder.Services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
            //payment
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            #endregion

            builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                //options.User.AllowedUserNameCharacters = "";
                //options.SignIn.RequireConfirmedEmail = true;
                //options.SignIn.RequireConfirmedPhoneNumber = true;
                //options.SignIn.RequireConfirmedAccount = true;

                //options.Password.RequiredLength = 6;
                //options.Password.RequireNonAlphanumeric = true;
                //options.Password.RequireLowercase = true;
                //options.Password.RequireUppercase = true;
                //options.Password.RequireDigit = true;
                //options.Password.RequiredUniqueChars = 2;

                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.AllowedForNewUsers = true;
            })
              .AddEntityFrameworkStores<IdentityDbContext>();
            builder.Services.AddScoped(typeof(IAuthService), typeof(AuthService));
            builder.Services.AddScoped(typeof(Func<IAuthService>), (serviceProvider) =>
            {
                return () => serviceProvider.GetService<IAuthService>();
            });

            builder.Services.AddAuthentication((options) => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer((options) =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!))
                    };
                });
            var app = builder.Build();
            using (var s = app.Services.CreateScope())
            {
                var service = s.ServiceProvider;
                await IdentitySeeding.SeedAsync(service);
            }
            var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<FixItDbContext>();
            var identityContext = services.GetRequiredService<IdentityDbContext>();
            var logger = services.GetRequiredService<ILogger<Program>>();
            try
            {
                await context.Database.MigrateAsync();
                await identityContext.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database.");
            }

            app.UseMiddleware<ExceptionHandlerMiddlewares>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("FixItPolicy");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
