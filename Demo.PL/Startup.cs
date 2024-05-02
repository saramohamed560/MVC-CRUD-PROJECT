using Demo.DAL.Data;
using Demo.DAL.Models;
using Demo.PL.Extensions;
using Demo.PL.Helper;
using Demp.BLL.Interfaces;
using Demp.BLL.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.PL
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
            services.AddControllersWithViews();  
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            }); 
            services.AddApplicationServices();  
            services.AddAutoMapper(M=>M.AddProfile(new MappingProfiles()));
			
			services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.Password.RequiredUniqueChars = 2;
                config.Password.RequireDigit = true;
                config.Password.RequireNonAlphanumeric = true; // @,#,....
                config.Password.RequiredLength = 5;
                config.Password.RequireUppercase = true; 
                config.Password.RequireLowercase = true;

                config.User.RequireUniqueEmail = true;
                config.Lockout.MaxFailedAccessAttempts = 3;//Available no of tries
                config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMilliseconds(1000);
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
           
            services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = "/Account/SignIn";
                config.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            });




        }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		// app => Kestrel
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection(); 
            app.UseStaticFiles(); 

            app.UseRouting(); 
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
