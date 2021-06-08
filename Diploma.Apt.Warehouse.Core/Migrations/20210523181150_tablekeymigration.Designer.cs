﻿// <auto-generated />
using System;
using Diploma.Apt.Warehouse.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

namespace Diploma.Apt.Warehouse.Core.Migrations
{
    [DbContext(typeof(WarehouseContext))]
    [Migration("20210523181150_tablekeymigration")]
    partial class tablekeymigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.6")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Diploma.Apt.Warehouse.Core.Data.Entities.PostgreSQL.BatchEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("BestBefore")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("Count")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsRecieved")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastUpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("ProviderName")
                        .HasColumnType("text");

                    b.Property<Guid>("StockId")
                        .HasColumnType("uuid");

                    b.Property<int>("TableKey")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.HasKey("Id");

                    b.HasIndex("StockId");

                    b.ToTable("Batches");
                });

            modelBuilder.Entity("Diploma.Apt.Warehouse.Core.Data.Entities.PostgreSQL.ProductEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastUpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("NameEn")
                        .HasColumnType("text");

                    b.Property<string>("NameUkr")
                        .HasColumnType("text");

                    b.Property<string>("ProductType")
                        .HasColumnType("text");

                    b.Property<NpgsqlTsVector>("SearchVector")
                        .HasColumnType("tsvector");

                    b.Property<int>("TableKey")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Diploma.Apt.Warehouse.Core.Data.Entities.PostgreSQL.StockEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Count")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("DepartmentId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("LastUpdatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("MaxCount")
                        .HasColumnType("integer");

                    b.Property<int?>("OrderPeriod")
                        .HasColumnType("integer");

                    b.Property<int?>("OrderPoint")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("OrderRepeat")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("PurchasePrice")
                        .HasColumnType("numeric");

                    b.Property<decimal>("SellPrice")
                        .HasColumnType("numeric");

                    b.Property<int>("TableKey")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("Stocks");
                });

            modelBuilder.Entity("Diploma.Apt.Warehouse.Core.Data.Entities.PostgreSQL.BatchEntity", b =>
                {
                    b.HasOne("Diploma.Apt.Warehouse.Core.Data.Entities.PostgreSQL.StockEntity", "Stock")
                        .WithMany()
                        .HasForeignKey("StockId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Stock");
                });

            modelBuilder.Entity("Diploma.Apt.Warehouse.Core.Data.Entities.PostgreSQL.StockEntity", b =>
                {
                    b.HasOne("Diploma.Apt.Warehouse.Core.Data.Entities.PostgreSQL.ProductEntity", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });
#pragma warning restore 612, 618
        }
    }
}
