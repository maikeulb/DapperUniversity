using System;
using Dapper.FluentMap;
using DapperUniversity.Data;
using DapperUniversity.Middlewares;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DapperUniversity
{
    public class Startup
    {
        public Startup (IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices (IServiceCollection services)
        {
            // Will incorporate Dapper provider
            /* services.AddDbContext<ApplicationDbContext>(options => */
            /*     options.UseNpgsql (Configuration.GetConnectionString ("Identity"))); */

            /* services.AddIdentity<ApplicationUser, IdentityRole>() */
            /*     .AddEntityFrameworkStores<ApplicationDbContext>() */
            /*     .AddDefaultTokenProviders(); */

            FluentMapper.Initialize (config =>
            {
                config.AddMap (new StudentMap ());
                config.AddMap (new CourseMap ());
                config.AddMap (new CourseAssignmentMap ());
                config.AddMap (new DepartmentMap ());
                config.AddMap (new EnrollmentMap ());
                config.AddMap (new InstructorMap ());
                config.AddMap (new OfficeAssignmentMap ());
            });

            services.AddMvc ()
                .AddFluentValidation (fv =>
                {
                    fv.RegisterValidatorsFromAssemblyContaining<Startup> ();
                    fv.ConfigureClientsideValidation (enabled: false);
                });

            string connectionString = Configuration.GetConnectionString ("DapperUniversity");

            if (connectionString == null)
                throw new ArgumentNullException ("Connection string cannot be null");
        }

        public void Configure (IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory factory)
        {
            app.UseMiddleware<ErrorLoggingMiddleware> ();

            if (env.IsDevelopment ())
            {
                app.UseDeveloperExceptionPage ();
                app.UseDatabaseErrorPage ();
            }
            else
            {
                app.UseExceptionHandler ("/Home/Error");
            }

            app.UseStaticFiles ();

            app.UseStatusCodePagesWithReExecute ("/StatusCode/{0}");

            app.UseMvc (routes =>
            {
                routes.MapRoute (
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}