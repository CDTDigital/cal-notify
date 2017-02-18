using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using CalNotify.Services;
using NpgsqlTypes;

namespace CalNotifyApi.Migrations
{
    [DbContext(typeof(BusinessDbContext))]
    [Migration("20170218005506_zipcode")]
    partial class zipcode
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:PostgresExtension:postgis", "'postgis', '', ''")
                .HasAnnotation("Npgsql:PostgresExtension:uuid-ossp", "'uuid-ossp', '', ''")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("CalNotify.Models.Addresses.Address", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City");

                    b.Property<string>("FormattedAddress");

                    b.Property<PostgisPoint>("GeoLocation");

                    b.Property<string>("Number");

                    b.Property<string>("State");

                    b.Property<string>("Street");

                    b.Property<Guid>("UserId");

                    b.Property<string>("Zip");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Address");
                });

            modelBuilder.Entity("CalNotify.Models.Admins.AdminConfiguration", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.ToTable("Configurations");
                });

            modelBuilder.Entity("CalNotify.Models.User.GenericUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<byte[]>("Avatar");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Email");

                    b.Property<string>("Enabled");

                    b.Property<DateTime>("JoinDate");

                    b.Property<DateTime>("LastLogin");

                    b.Property<string>("Name");

                    b.Property<string>("Password");

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("AllUsers");

                    b.HasDiscriminator<string>("Discriminator").HasValue("GenericUser");
                });

            modelBuilder.Entity("CalNotify.Services.ZipCodeInfo", b =>
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

            modelBuilder.Entity("CalNotify.Models.Admins.WebAdmin", b =>
                {
                    b.HasBaseType("CalNotify.Models.User.GenericUser");


                    b.ToTable("WebAdmin");

                    b.HasDiscriminator().HasValue("WebAdmin");
                });

            modelBuilder.Entity("CalNotify.Models.Addresses.Address", b =>
                {
                    b.HasOne("CalNotify.Models.User.GenericUser", "User")
                        .WithOne("Address")
                        .HasForeignKey("CalNotify.Models.Addresses.Address", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
