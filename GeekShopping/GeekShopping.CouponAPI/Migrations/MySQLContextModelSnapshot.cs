﻿// <auto-generated />
using GeekShopping.CouponAPI.Model.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GeekShopping.CouponAPI.Migrations
{
    [DbContext(typeof(MySQLContext))]
    partial class MySQLContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("GeekShopping.CouponAPI.Model.Coupon", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("CouponCode")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)")
                        .HasColumnName("coupon_code");

                    b.Property<decimal>("DiscountAmount")
                        .HasColumnType("decimal(65,30)")
                        .HasColumnName("discount_amount");

                    b.HasKey("Id");

                    b.ToTable("coupon");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            CouponCode = "ANDRE_2024_12",
                            DiscountAmount = 10m
                        },
                        new
                        {
                            Id = 2L,
                            CouponCode = "ANDRE_2024_05",
                            DiscountAmount = 15m
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
