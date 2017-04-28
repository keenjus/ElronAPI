using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ElronAPI.Models;

namespace ElronAPI.Migrations
{
    [DbContext(typeof(elronContext))]
    partial class elronContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("ElronAPI.Models.CachedAccount", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("json");

                    b.Property<DateTime>("ExpireTime");

                    b.HasKey("Id");

                    b.ToTable("CachedAccounts");
                });
        }
    }
}
