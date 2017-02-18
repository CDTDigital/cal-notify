using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using CalNotify.Models.Addresses;
using CalNotify.Models.Admins;
using CalNotify.Models.User;
using CalNotifyApi.Models.Admins;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using NpgsqlTypes;


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

        public DbSet<ZipCodeInfo> ZipCodes { get; set; }

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
            builder.HasPostgresExtension("postgis");
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


        public static async Task<bool> SeedDatabase(this BusinessDbContext context, string contentRoot, bool isDevel, IServiceProvider services)
        {
            var path = Path.Combine(contentRoot, "zipcodes.csv");
            var file = File.OpenRead(path);
            var csv = new CsvReader(new StreamReader(file));
            csv.Configuration.RegisterClassMap<ZipCodeInfoMap>();
            foreach (var record in csv.GetRecords<ZipCodeInfo>())
            {
              
                if (context.ZipCodes.FirstOrDefault(zip => zip.Zipcode == record.Zipcode) == null)
                {
                    context.ZipCodes.Add(record);
                }
             
            }
            foreach (var adminEvent in Constants.Testing.TestAdmins)
            {

                // wont create if already in place
                adminEvent.Process(context);
            }

            return true;
        }
    }

    [DataContract]
    public class ZipCodeInfo
    {
       
        [Key]
        public string Zipcode { get; set; }


        public string City { get; set; }


        public string County { get; set; }


        public string Region { get; set; }
        public PostgisPoint Location { get; set; }

        
     
    }

    public sealed class ZipCodeInfoMap : CsvClassMap<ZipCodeInfo>
    {
        public ZipCodeInfoMap()
        {
            Map(z => z.Zipcode).Name("Zipcode");
            Map(z => z.City).Name("City");
            Map(z => z.Region).Name("Region");
            Map(x => x.County).Name("County");
            Map(x => x.Location).ConvertUsing(row => new PostgisPoint(row.GetField<double>("Latitude"), row.GetField<double>("Longitude")) {SRID = Constants.SRID});
          

        }
    }

   
}
