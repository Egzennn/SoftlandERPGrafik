﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SoftlandERPGrafik.Data.DB;

#nullable disable

namespace SoftlandERPGrafik.Data.Migrations
{
    [DbContext(typeof(MainContext))]
    [Migration("20240305111626_WnioskiFormv2")]
    partial class WnioskiFormv2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SoftlandERPGrafik.Data.Entities.Forms.GrafikForm", b =>
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

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsAllDay")
                        .HasColumnType("bit");

                    b.Property<int?>("LocationId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<int>("PRI_PraId")
                        .HasColumnType("int");

                    b.Property<string>("RecurrenceException")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("RecurrenceID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("RecurrenceRule")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Stan")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("GrafikForms");
                });

            modelBuilder.Entity("SoftlandERPGrafik.Data.Entities.Forms.WnioskiForm", b =>
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

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsAllDay")
                        .HasColumnType("bit");

                    b.Property<int>("PRI_PraId")
                        .HasColumnType("int");

                    b.Property<string>("RecurrenceException")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("RecurrenceID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("RecurrenceRule")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RequestId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("Stan")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("WnioskiForms");
                });

            modelBuilder.Entity("SoftlandERPGrafik.Data.Entities.Views.Kierownicy", b =>
                {
                    b.Property<string>("CNT_Nazwa")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PRI_Imie1")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PRI_Nazwisko")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PRI_Opis")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PRI_PraId")
                        .HasColumnType("int");

                    b.ToTable("Kierownicy");
                });

            modelBuilder.Entity("SoftlandERPGrafik.Data.Entities.Views.OrganizacjaLokalizacje", b =>
                {
                    b.Property<int>("Lok_LokId")
                        .HasColumnType("int");

                    b.Property<string>("Wartosc")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("OrganizacjaLokalizacjeVocabulary");
                });

            modelBuilder.Entity("SoftlandERPGrafik.Data.Entities.Views.ZatrudnieniDzialy", b =>
                {
                    b.Property<int>("DZL_DzlId")
                        .HasColumnType("int");

                    b.Property<string>("DZL_Kod")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("ZatrudnieniDzialy");
                });

            modelBuilder.Entity("SoftlandERPGrafik.Data.Entities.Views.ZatrudnieniZrodlo", b =>
                {
                    b.Property<int?>("DZL_DzlId")
                        .HasColumnType("int");

                    b.Property<string>("DZL_Kod")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Imie_Nazwisko")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PRE_Plec")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PRI_Imie1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PRI_Kod")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PRI_Nazwisko")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PRI_Opis")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PRI_PraId")
                        .HasColumnType("int");

                    b.ToTable("ZatrudnieniZrodlo");
                });

            modelBuilder.Entity("SoftlandERPGrafik.Data.Entities.Vocabularies.Forms.Ogolne.OgolneStan", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Odpowiedzialny")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Stan")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Wartosc")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("OgolneStanVocabulary");
                });

            modelBuilder.Entity("SoftlandERPGrafik.Data.Entities.Vocabularies.Forms.Ogolne.OgolneStatus", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Odpowiedzialny")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Stan")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Wartosc")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("OgolneStatusVocabulary");
                });

            modelBuilder.Entity("SoftlandERPGrafik.Data.Entities.Vocabularies.Forms.Ogolne.OgolneWnioski", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Odpowiedzialny")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Stan")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Wartosc")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("OgolneWnioskiVocabulary");
                });
#pragma warning restore 612, 618
        }
    }
}