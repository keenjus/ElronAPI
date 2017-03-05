using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ElronAPI.Models;

namespace ElronAPI.Migrations
{
    [DbContext(typeof(ApplicationDb))]
    [Migration("20170305120500_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ElronAPI.Models.ElronAccount", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("ActivePeriodTicketId");

                    b.Property<decimal?>("Balance");

                    b.Property<DateTime>("LastCheck");

                    b.HasKey("Id");

                    b.HasIndex("ActivePeriodTicketId");

                    b.ToTable("ElronAccounts");
                });

            modelBuilder.Entity("ElronAPI.Models.ElronPeriodTicket", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ElronAccountId");

                    b.Property<long>("TransactionId");

                    b.Property<DateTime>("ValidFrom");

                    b.Property<DateTime>("ValidTo");

                    b.HasKey("Id");

                    b.HasIndex("ElronAccountId");

                    b.HasIndex("TransactionId");

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
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<string>("ElronAccountId");

                    b.Property<string>("Name");

                    b.Property<decimal>("Sum");

                    b.Property<Guid?>("TicketId");

                    b.HasKey("Id");

                    b.HasIndex("ElronAccountId");

                    b.HasIndex("TicketId");

                    b.ToTable("ElronTransaction");
                });

            modelBuilder.Entity("ElronAPI.Models.ElronAccount", b =>
                {
                    b.HasOne("ElronAPI.Models.ElronPeriodTicket", "ActivePeriodTicket")
                        .WithMany()
                        .HasForeignKey("ActivePeriodTicketId");
                });

            modelBuilder.Entity("ElronAPI.Models.ElronPeriodTicket", b =>
                {
                    b.HasOne("ElronAPI.Models.ElronAccount")
                        .WithMany("PeriodTickets")
                        .HasForeignKey("ElronAccountId");

                    b.HasOne("ElronAPI.Models.ElronTransaction", "Transaction")
                        .WithMany()
                        .HasForeignKey("TransactionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElronAPI.Models.ElronTransaction", b =>
                {
                    b.HasOne("ElronAPI.Models.ElronAccount")
                        .WithMany("Transactions")
                        .HasForeignKey("ElronAccountId");

                    b.HasOne("ElronAPI.Models.ElronTicket", "Ticket")
                        .WithMany()
                        .HasForeignKey("TicketId");
                });
        }
    }
}
