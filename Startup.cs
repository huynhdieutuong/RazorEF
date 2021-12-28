using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RazorEF.Models;

namespace RazorEF
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddDbContext<MyBlogContext>(options =>
            {
                string connectString = Configuration.GetConnectionString("MyBlogContext");
                options.UseSqlServer(connectString);
            });
            services.AddSingleton<IEmailSender, SendMailService>(); // 9.5 Inject SendMailService & Identity can send mail

            // 9.2 Inject MailSettings to Options
            services.AddOptions();
            var mailSettings = Configuration.GetSection("MailSettings");
            services.Configure<MailSettings>(mailSettings);

            // 4. Register Identity
            services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<MyBlogContext>()
                    .AddDefaultTokenProviders();

            // 8.1 Use AddDefaultIdentity for Identity UI Default
            // services.AddDefaultIdentity<AppUser>()
            //         .AddEntityFrameworkStores<MyBlogContext>()
            //         .AddDefaultTokenProviders();

            // 7. Custome config for Identity:
            // Access IdentityOptions
            services.Configure<IdentityOptions>(options =>
            {
                // Password
                options.Password.RequireDigit = false; // Not require number
                options.Password.RequireLowercase = false; // Not require lowercase
                options.Password.RequireNonAlphanumeric = false; // Not require special characters
                options.Password.RequireUppercase = false; // Not require uppercase
                options.Password.RequiredLength = 3; // Minimum 3 chars
                options.Password.RequiredUniqueChars = 1; // Unique chars

                // Lock user if user login fail more than 5 times in 5 minutes
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Lock 5 minutes
                options.Lockout.MaxFailedAccessAttempts = 5; // Login fail 5 times
                options.Lockout.AllowedForNewUsers = true;

                // Create User
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; // UserName only include these chars
                options.User.RequireUniqueEmail = true;  // Email is unique

                // Login
                options.SignIn.RequireConfirmedEmail = true; // Confirm email is exists
                options.SignIn.RequireConfirmedPhoneNumber = false; // Confirm phone number
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // 1. Make sure add 2 middlewares:
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
