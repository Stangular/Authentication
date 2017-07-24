
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AuthUser.Data.Entities;

namespace AuthUser.Data
{
    public class UserIdentityContext : IdentityDbContext
    {
        private IConfigurationRoot _config;

        public UserIdentityContext(DbContextOptions options, IConfigurationRoot config)
          : base(options)
        {
            _config = config;
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

        
           
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);

        //    optionsBuilder.UseSqlServer(_config["Data:ConnectionString"]);
        //}
    }
}

