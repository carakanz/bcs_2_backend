using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Backend
{
    public class Startup
    {
        public class AppSettings
        {
            public string ValidIssuer { get; set; }
            public string ValidAudience { get; set; }
            public SymmetricSecurityKey IssuerSigningKey { get; set; }
            public TimeSpan Expires { get; set; }
        }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<Models.IdentityApplicationContext>(options =>
               options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers();

            var authOptions = Configuration.GetSection("AuthOptions");

            var appSettings = new AppSettings
            {
                IssuerSigningKey = new SymmetricSecurityKey(
                                    Encoding.ASCII.GetBytes(
                                        authOptions.GetValue<string>("Key") + "carakan28831")),
                ValidAudience = authOptions.GetValue<string>("ValidAudience"),
                ValidIssuer = authOptions.GetValue<string>("ValidIssuer"),
                Expires = TimeSpan.FromMinutes(authOptions.GetValue<double>("LifeTime"))
            };

            services.Configure<AppSettings>(opt =>
            {
                opt.Expires = appSettings.Expires;
                opt.IssuerSigningKey = appSettings.IssuerSigningKey;
                opt.ValidAudience = appSettings.ValidAudience;
                opt.ValidIssuer = appSettings.ValidIssuer;
            });

            services.AddIdentity<Models.DBUser, IdentityRole>(options =>
            {
                // Password settings.
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false; 
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = false;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<Models.IdentityApplicationContext>();
            services.TryAddScoped<RoleManager<IdentityRole>>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // укзывает, будет ли валидироваться издатель при валидации токена
                    ValidateIssuer = authOptions.GetValue<bool>("ValidateIssuer"),
                    // строка, представляющая издателя
                    ValidIssuer = appSettings.ValidIssuer,

                    // будет ли валидироваться потребитель токена
                    ValidateAudience = authOptions.GetValue<bool>("ValidateAudience"),
                    // установка потребителя токена
                    ValidAudience = appSettings.ValidAudience,
                    // будет ли валидироваться время существования
                    ValidateLifetime = authOptions.GetValue<bool>("ValidateLifetime"),

                    // установка ключа безопасности
                    IssuerSigningKey = appSettings.IssuerSigningKey,
                    // валидация ключа безопасности
                    ValidateIssuerSigningKey = authOptions.GetValue<bool>("ValidateIssuerSigningKey"),
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
