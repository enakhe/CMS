﻿// <auto-generated />
using System;
using CMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CMS.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CMS.Domain.Entities.Criminal", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CriminalID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NationalID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Offenses")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("WatchlistStatus")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Criminals");
                });

            modelBuilder.Entity("CMS.Domain.Entities.CriminalBiometrics", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CriminalId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DNAProfile")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("FingerprintData")
                        .HasColumnType("varbinary(max)");

                    b.HasKey("Id");

                    b.HasIndex("CriminalId");

                    b.ToTable("CriminalBiometrics");
                });

            modelBuilder.Entity("CMS.Domain.Entities.CriminalPictures", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.PrimitiveCollection<string>("AdditionalPictures")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CriminalId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("Mugshot")
                        .HasColumnType("varbinary(max)");

                    b.HasKey("Id");

                    b.HasIndex("CriminalId");

                    b.ToTable("CriminalPictures");
                });

            modelBuilder.Entity("CMS.Domain.Entities.CriminalBiometrics", b =>
                {
                    b.HasOne("CMS.Domain.Entities.Criminal", "Criminal")
                        .WithMany()
                        .HasForeignKey("CriminalId");

                    b.Navigation("Criminal");
                });

            modelBuilder.Entity("CMS.Domain.Entities.CriminalPictures", b =>
                {
                    b.HasOne("CMS.Domain.Entities.Criminal", "Criminal")
                        .WithMany("Pictures")
                        .HasForeignKey("CriminalId");

                    b.Navigation("Criminal");
                });

            modelBuilder.Entity("CMS.Domain.Entities.Criminal", b =>
                {
                    b.Navigation("Pictures");
                });
#pragma warning restore 612, 618
        }
    }
}
