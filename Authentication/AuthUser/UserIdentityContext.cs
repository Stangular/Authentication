using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthUser
{
    public class UserIdentityContext : IdentityDbContext<AUser>
    {
        //  public virtual DbSet<IdentityUser> IdentityUser { get; set; }
        public DbSet<AdditionalUserInformation> AdditionalUserInformation { get; set; }
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

        //    optionsBuilder.UseSqlServer("Server=OFFICE_STAN\\SQLEXPRESS;Database=UserIdentityB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        //}
    }
}

