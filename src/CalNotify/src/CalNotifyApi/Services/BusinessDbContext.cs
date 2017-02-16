using System;
using System.Linq;
using System.Threading.Tasks;
using CalNotify.Models.Addresses;
using CalNotify.Models.Admins;
using CalNotify.Models.User;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace CalNotify.Services
{
    public class BusinessDbContext : DbContext
    {
        #region Constructors



        public BusinessDbContext(DbContextOptions<BusinessDbContext> options)
            : base(options)
        {

        }


        #endregion

        #region Entities

        public DbSet<GenericUser> AllUsers { get; set; }
       
        public DbSet<Address> Address { get; set; }
        private DbSet<AdminConfiguration> Configurations { get; set; }

        public AdminConfiguration AdminConfig
        {
            get
            {
                // only ever return singleton
                var config = Configurations.FirstOrDefault(x => x.Id == 1);
                if (config != null)
                {
                    return config;
                }
                config = new AdminConfiguration();
                Configurations.Add(config);
                this.SaveChanges();
                return config;
            }
        }
  

        public IQueryable<WebAdmin> Admins => this.AllUsers.OfType<WebAdmin>();
        public IQueryable<GenericUser> Users => this.AllUsers.OfType<GenericUser>();

        #endregion
        protected override void OnModelCreating(ModelBuilder builder)
        {


            base.OnModelCreating(builder);

            builder.HasPostgresExtension("uuid-ossp");

            builder.Entity<WebAdmin>();

            builder
            .Entity<GenericUser>()
            .Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()");
     

        }



    }


    public static class BusinessDbExtensions
    {

    
        public static async Task<bool> SeedDatabase( this BusinessDbContext context, string contentRoot, bool isDevel, IServiceProvider services)
        {
            foreach (var adminEvent in Constants.Testing.TestAdmins)
            {
                // wont create if already in place
                adminEvent.Process(context);
            }

            return true;
        }


    }
}
