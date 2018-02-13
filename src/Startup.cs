using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DapperUniversity.Data;
using DapperUniversity.Models;
using DapperUniversity.Services;
using Dapper.FluentMap;
using MediatR;
using FluentValidation.AspNetCore;

namespace DapperUniversity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql (Configuration.GetConnectionString ("Identity")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAutoMapper();

            FluentMapper.Initialize(config =>
                {
                   config.AddMap(new StudentMap());
                   config.AddMap(new CourseMap());
                   config.AddMap(new CourseAssignmentMap());
                   config.AddMap(new DepartmentMap());
                   config.AddMap(new EnrollmentMap());
                   config.AddMap(new InstructorMap());
                   config.AddMap(new OfficeAssignmentMap());
                });

            services.AddMediatR();

            services.AddMvc();

            string connectionString = Configuration.GetConnectionString ("DapperUniversity");

            if (connectionString == null)
              throw new ArgumentNullException("Connection string cannot be null");

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
