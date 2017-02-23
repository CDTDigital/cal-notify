﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using CalNotifyApi.Services;
using NpgsqlTypes;

namespace CalNotifyApi.Migrations
{
    [DbContext(typeof(BusinessDbContext))]
    [Migration("20170221230351_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:PostgresExtension:postgis", "'postgis', '', ''")
                .HasAnnotation("Npgsql:PostgresExtension:uuid-ossp", "'uuid-ossp', '', ''")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("CalNotifyApi.Models.Addresses.Address", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City")
                        .IsRequired();

                    b.Property<string>("FormattedAddress");

                    b.Property<PostgisPoint>("GeoLocation");

                    b.Property<string>("Number")
                        .IsRequired();

                    b.Property<string>("State")
                        .IsRequired();

                    b.Property<string>("Street")
                        .IsRequired();

                    b.Property<Guid>("UserId");

                    b.Property<string>("Zip")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Address");
                });

            modelBuilder.Entity("CalNotifyApi.Models.Admins.AdminConfiguration", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.ToTable("Configurations");
                });

            modelBuilder.Entity("CalNotifyApi.Models.BaseUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<byte[]>("Avatar");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Email");

                    b.Property<bool>("Enabled");

                    b.Property<DateTime>("JoinDate");

                    b.Property<DateTime>("LastLogin");

                    b.Property<string>("Name");

                    b.Property<string>("Password");

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("UserName");

                    b.Property<bool>("ValidatedEmail");

                    b.Property<bool>("ValidatedSms");

                    b.HasKey("Id");

                    b.ToTable("AllUsers");

                    b.HasDiscriminator<string>("Discriminator").HasValue("BaseUser");
                });

            modelBuilder.Entity("CalNotifyApi.Services.ZipCodeInfo", b =>
                {
                    b.Property<string>("Zipcode")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City");

                    b.Property<string>("County");

                    b.Property<PostgisPoint>("Location");

                    b.Property<string>("Region");

                    b.HasKey("Zipcode");

                    b.ToTable("ZipCodes");
                });

            modelBuilder.Entity("CalNotifyApi.Models.Admins.WebAdmin", b =>
                {
                    b.HasBaseType("CalNotifyApi.Models.BaseUser");


                    b.ToTable("WebAdmin");

                    b.HasDiscriminator().HasValue("WebAdmin");
                });

            modelBuilder.Entity("CalNotifyApi.Models.GenericUser", b =>
                {
                    b.HasBaseType("CalNotifyApi.Models.BaseUser");


                    b.ToTable("GenericUser");

                    b.HasDiscriminator().HasValue("GenericUser");
                });

            modelBuilder.Entity("CalNotifyApi.Models.Addresses.Address", b =>
                {
                    b.HasOne("CalNotifyApi.Models.GenericUser", "User")
                        .WithOne("Address")
                        .HasForeignKey("CalNotifyApi.Models.Addresses.Address", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
