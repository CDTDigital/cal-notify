﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CalNotifyApi.Models;
using CalNotifyApi.Models.Addresses;
using CalNotifyApi.Models.Admins;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using NpgsqlTypes;

namespace CalNotifyApi.Services
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

        /// <summary>
        /// Reference all the user rows stored in our databse
        /// </summary>
        public DbSet<BaseUser> AllUsers { get; set; }

        /// <summary>
        /// Refernce to the complete set of addresses stored in our system
        /// </summary>
        public DbSet<Address> Address { get; set; }
        /// <summary>
        /// Provides access to a single row which holds our configurable properties for our prototype
        /// </summary>
        private DbSet<AdminConfiguration> Configurations { get; set; }


        public DbSet<BroadCastLogEntry> NotificationLog { get; set; }


        /// <summary>
        /// List of notifications in the system
        /// </summary>
        public DbSet<Notification> Notifications { get; set; }

        /// <summary>
        /// our configurable properties for our prototype
        /// </summary>
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


        /// <summary>
        /// Gets the subset of our admins which are enabled within our system.
        /// </summary>
        public IQueryable<WebAdmin> Admins => this.AllUsers.OfType<WebAdmin>()
            .Where(u => u.Enabled);

        /// <summary>
        /// Gets the subset of our users which are enabled within our system.
        /// In addition we explictly include our users address here...
        /// TODO: Performance improvement via selectively including(joining) the address table only when need be
        /// </summary>
        public IQueryable<GenericUser> Users => this.AllUsers.OfType<GenericUser>()
            .Include(u => u.Address)
            .Where(u => u.Enabled);

        #endregion
        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.HasPostgresExtension("postgis");
            builder.HasPostgresExtension("uuid-ossp");
            base.OnModelCreating(builder);

            builder.Entity<Notification>();
            builder.Entity<BaseUser>();
            builder.Entity<WebAdmin>();
            builder.Entity<GenericUser>();

            builder
            .Entity<GenericUser>()
            .Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()");


           // builder.Entity<BroadCastLogEntry>().HasKey(x => new {x.Id});

        }
    }

    /// <summary>
    /// Helpers to populate and manipulate our Database reference
    /// </summary>
    public static class BusinessDbExtensions
    {

        /// <summary>
        /// Populates our databases tables with the required initial rows, if missing
        /// </summary>
        /// <param name="context"></param>
        /// <param name="contentRoot"></param>
        /// <param name="isDevel"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        public static async Task<bool> SeedDatabase(this BusinessDbContext context, string contentRoot, bool isDevel, IServiceProvider services)
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
