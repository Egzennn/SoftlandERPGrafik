﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SoftlandERPGrafik.Data.DB;

#nullable disable

namespace SoftlandERPGrafik.Data.Migrations.Schedule
{
    [DbContext(typeof(ScheduleContext))]
    partial class ScheduleContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SoftlandERPGrafik.Data.Entities.Forms.Data.Holidays", b =>
                {
                    b.Property<DateTime>("Data")
                        .HasColumnType("datetime2");

                    b.Property<int>("Dzien")
                        .HasColumnType("int");

                    b.Property<string>("DzienTyg")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Mies")
                        .HasColumnType("int");

                    b.Property<string>("Nazwa")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Rodzaj")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Rok")
                        .HasColumnType("int");

                    b.HasKey("Data");

                    b.ToTable("HolidaysData");
                });

            modelBuilder.Entity("SoftlandERPGrafik.Data.Entities.Forms.ScheduleForm", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DZL_DzlId")
                        .HasColumnType("int");

                    b.Property<int?>("DaysAmount")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("IDD")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IDS")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAllDay")
                        .HasColumnType("bit");

                    b.Property<int?>("LocationId")
                        .HasColumnType("int");

                    b.Property<int>("PRI_PraId")
                        .HasColumnType("int");

                    b.Property<string>("RecurrenceException")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("RecurrenceID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("RecurrenceRule")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("RequestId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Stan")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ScheduleForms");
                });
#pragma warning restore 612, 618
        }
    }
}
