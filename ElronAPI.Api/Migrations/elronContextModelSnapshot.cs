using System;
using ElronAPI.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ElronAPI.Api.Migrations
{
    [DbContext(typeof(ElronContext))]
    partial class elronContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("ElronAPI.Models.CachedResponse", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<DateTime>("ExpireTime");

                    b.HasKey("Id");

                    b.ToTable("CachedResponses");
                });
        }
    }
}
