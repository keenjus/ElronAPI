using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElronAPI.Models
{
    public class ElronAccount
    {
        public ElronAccount()
        {
            Transactions = new List<ElronTransaction>();
            PeriodTickets = new List<ElronPeriodTicket>();
        }

        [Key]
        public string Id { get; set; }
        public decimal? Balance { get; set; }
        public ElronPeriodTicket ActivePeriodTicket { get; set; }
        public List<ElronPeriodTicket> PeriodTickets { get; set; }
        public List<ElronTransaction> Transactions { get; set; }
        public DateTime LastCheck { get; set; }
    }

    public class ElronTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public ElronTicket Ticket { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public decimal Sum { get; set; }
    }

    public class ElronTicket
    {
        [Key]
        public Guid Id { get; set; }
        public string Number { get; set; }
        public string Url { get; set; }
    }

    public class ElronPeriodTicket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public ElronTicket Ticket { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
}