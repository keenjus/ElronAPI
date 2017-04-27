using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ElronAPI.Models;

namespace ElronAPI.Migrations.elron
{
    [DbContext(typeof(elronContext))]
    partial class elronContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("ElronAPI.Models.ElronAccount", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal?>("Balance");

                    b.Property<decimal?>("BalanceThreshold");

                    b.Property<DateTime>("LastCheck");

                    b.Property<int?>("PeriodTicketThreshold");

                    b.HasKey("Id");

                    b.ToTable("ElronAccount");
                });

            modelBuilder.Entity("ElronAPI.Models.ElronPeriodTicket", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<string>("ElronAccountId");

                    b.Property<DateTime>("ValidFrom");

                    b.Property<DateTime>("ValidTo");

                    b.HasKey("Id");

                    b.HasIndex("ElronAccountId");

                    b.ToTable("ElronPeriodTicket");
                });

            modelBuilder.Entity("ElronAPI.Models.ElronTicket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Number");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("ElronTicket");
                });

            modelBuilder.Entity("ElronAPI.Models.ElronTransaction", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<DateTime>("Date");

                    b.Property<string>("ElronAccountId");

                    b.Property<string>("Name");

                    b.Property<long?>("PeriodTicketId");

                    b.Property<decimal>("Sum");

                    b.Property<Guid?>("TicketId");

                    b.HasKey("Id");

                    b.HasIndex("ElronAccountId");

                    b.HasIndex("PeriodTicketId");

                    b.HasIndex("TicketId");

                    b.ToTable("ElronTransaction");
                });

            modelBuilder.Entity("ElronAPI.Models.ElronPeriodTicket", b =>
                {
                    b.HasOne("ElronAPI.Models.ElronAccount")
                        .WithMany("PeriodTickets")
                        .HasForeignKey("ElronAccountId");
                });

            modelBuilder.Entity("ElronAPI.Models.ElronTransaction", b =>
                {
                    b.HasOne("ElronAPI.Models.ElronAccount")
                        .WithMany("Transactions")
                        .HasForeignKey("ElronAccountId");

                    b.HasOne("ElronAPI.Models.ElronPeriodTicket", "PeriodTicket")
                        .WithMany()
                        .HasForeignKey("PeriodTicketId");

                    b.HasOne("ElronAPI.Models.ElronTicket", "Ticket")
                        .WithMany()
                        .HasForeignKey("TicketId");
                });
        }
    }
}
