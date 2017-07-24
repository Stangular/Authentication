using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AuthUser;

namespace UserLoginMirror
{
    public class Startup
    {
        IConfigurationRoot _config { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            _config = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_config);
            // Add framework services.
            var connection = _config.GetConnectionString("IdentityDB");// @"Server=OFFICE_STAN\SQLEXPRESS;Database=mySchedule;Integrated Security=True;MultipleActiveResultSets=true";
            services.AddDbContext<UserIdentityContext>(options => options.UseSqlServer(connection, b => b.MigrationsAssembly("UserRegistration")));
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<UserIdentityContext>(); ;

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(_config.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIdentity();

            app.UseMvc();
        }
    }
}
