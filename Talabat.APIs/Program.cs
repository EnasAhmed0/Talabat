using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.APIs.Helpers;
using Talabat.APIs.MiddleWares;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repository;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;

namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configure Services Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<StoreDbContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddDbContext<AppIdentityDbContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>(Options =>
            {
                var Connection = builder.Configuration.GetConnectionString("RedisConnection");
                return ConnectionMultiplexer.Connect(Connection);
            });
            builder.Services.AddApplicationServices();
            builder.Services.AddIdentityService(builder.Configuration);
            builder.Services.AddCors(Options =>
            {
                Options.AddPolicy("MyPolicy", option =>
                {
                    option.AllowAnyHeader();
                    option.AllowAnyMethod();
                    option.WithOrigins(builder.Configuration["FrontBaseUrl"]);
                });
            });
            #endregion

            var app = builder.Build();

            #region Update DataBase Explicitly
            //StoreDbContext dbContext = new StoreDbContext();//Invalid
            //await dbContext.Database.MigrateAsync();

            //Group of Services Which Liftime Is Scoped
            using var scope = app.Services.CreateScope();
            //Catch the Services Itself
            var services = scope.ServiceProvider;
            //Ask CLR To Create an Object Form ILoggerFactory Explicitly
            var LoggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                //Ask CLR To Create an Object Form Dbcontext Explicitly
                var Dbcontext = services.GetRequiredService<StoreDbContext>();
                await Dbcontext.Database.MigrateAsync();//Update Business Database
                var IdentityDbcontext = services.GetRequiredService<AppIdentityDbContext>();
                await IdentityDbcontext.Database.MigrateAsync();//Update Identity Database
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                await AppIdentityDbContextSeed.SeedUserAsync(userManager);
                await StoreContextSeed.SeedAsync(Dbcontext);
            }
            catch (Exception ex)
            {
                var Logger = LoggerFactory.CreateLogger<Program>();
                Logger.LogError(ex, "Error Occured During Update DataBase In its Region in Program File");
            }
            #endregion

            #region Configure middleware the HTTP request pipeline

            if (app.Environment.IsDevelopment())
            {
                app.UseMiddleware<ExceptionMiddleware>();
                app.UseSwaggerMiddleWares();
            }
            app.UseStaticFiles();
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseHttpsRedirection();
            app.UseCors("MyPolicy");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers(); 
            #endregion

            app.Run();
        }
    }
}
