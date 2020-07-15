﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SellingSolutions.Models;

namespace SellingSolutions.Migrations.SellingSolutions
{
    [DbContext(typeof(SellingSolutionsContext))]
    [Migration("20190904045944_Create_Sales_Cart_Schema")]
    partial class Create_Sales_Cart_Schema
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SellingSolutions.Models.Cart", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CartID");

                    b.Property<int>("ItemID");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<decimal>("PriceGST")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<decimal>("PriceTotal")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int>("Quantity");

                    b.HasKey("ID");

                    b.ToTable("Cart");
                });

            modelBuilder.Entity("SellingSolutions.Models.Product", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Category")
                        .IsRequired();

                    b.Property<string>("Image")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int>("Quantity");

                    b.Property<string>("Seller");

                    b.HasKey("ID");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("SellingSolutions.Models.Sales", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Buyer")
                        .IsRequired();

                    b.Property<decimal>("Cost")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int>("ItemID");

                    b.Property<DateTime>("PurchaseDate");

                    b.Property<int>("Quantity");

                    b.HasKey("ID");

                    b.ToTable("Sales");
                });
#pragma warning restore 612, 618
        }
    }
}
