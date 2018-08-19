using System;
using System.Collections.Generic;

namespace ElronAPI.Api.Models
{
    public class ElronAccountModel
    {
        public ElronAccountModel()
        {
            Transactions = new HashSet<Transaction>();
            PeriodTickets = new HashSet<PeriodTicket>();
        }

        public string Id { get; set; }
        public decimal? Balance { get; set; }
        public int? PeriodTicketThreshold { get; set; }
        public decimal? BalanceThreshold { get; set; }
        public DateTime LastCheck { get; set; }
        public ICollection<PeriodTicket> PeriodTickets { get; set; }
        public ICollection<Transaction> Transactions { get; set; }

        public class Transaction
        {
            public DateTime Date { get; set; }
            public string Name { get; set; }
            public decimal Sum { get; set; }

            public virtual Ticket Ticket { get; set; }
            public virtual PeriodTicket PeriodTicket { get; set; }
        }

        public class Ticket
        {
            public Guid Id { get; set; }
            public string Number { get; set; }
            public string Url { get; set; }
        }

        public class PeriodTicket
        {
            public DateTime ValidFrom { get; set; }
            public DateTime ValidTo { get; set; }

            public bool IsActive => ValidTo > DateTime.Now;
        }
    }
}