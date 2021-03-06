﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using CalNotifyApi.Services;
using NpgsqlTypes;
using CalNotifyApi.Models;

namespace CalNotifyApi.Migrations
{
    [DbContext(typeof(BusinessDbContext))]
    partial class BusinessDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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
                        .HasMaxLength(80);

                    b.Property<string>("FormattedAddress")
                        .HasMaxLength(80);

                    b.Property<PostgisPoint>("GeoLocation");

                    b.Property<string>("Number")
                        .HasMaxLength(80);

                    b.Property<string>("State")
                        .HasMaxLength(80);

                    b.Property<string>("Street")
                        .HasMaxLength(80);

                    b.Property<Guid>("UserId");

                    b.Property<string>("Zip")
                        .HasMaxLength(80);

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

                    b.Property<bool>("EnabledEmail");

                    b.Property<bool>("EnabledSms");

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

            modelBuilder.Entity("CalNotifyApi.Models.BroadCastLogEntry", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("NotificationId");

                    b.Property<int>("Type");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.ToTable("NotificationLog");
                });

            modelBuilder.Entity("CalNotifyApi.Models.Notification", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<PostgisPolygon>("AffectedArea");

                    b.Property<Guid>("AuthorId");

                    b.Property<int>("Category");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Details")
                        .IsRequired();

                    b.Property<PostgisPoint>("Location");

                    b.Property<DateTime?>("Published");

                    b.Property<Guid?>("PublishedById");

                    b.Property<int>("Severity");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("SourceId");

                    b.Property<int>("Status");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("PublishedById");

                    b.ToTable("Notifications");
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

            modelBuilder.Entity("CalNotifyApi.Models.Notification", b =>
                {
                    b.HasOne("CalNotifyApi.Models.Admins.WebAdmin", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CalNotifyApi.Models.Admins.WebAdmin", "PublishedBy")
                        .WithMany()
                        .HasForeignKey("PublishedById");
                });
        }
    }
}
