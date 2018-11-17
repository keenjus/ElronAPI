using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ElronAPI.Domain.Models
{
    public class ElronAccountModel
    {
        public ElronAccountModel()
        {
            Transactions = new List<Transaction>();
        }

        public string Id { get; set; }
        public decimal? Balance { get; set; }

        [JsonIgnore]
        public int? PeriodTicketThreshold { get; set; }

        [JsonIgnore]
        public decimal? BalanceThreshold { get; set; }

        public DateTime LastCheck { get; set; }
        public IEnumerable<PeriodTicket> PeriodTickets => Transactions.Where(x => x.PeriodTicket != null).Select(x => x.PeriodTicket).ToList();
        public List<Transaction> Transactions { get; set; }

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