using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElronAPI.Api.Data
{
    public class ElronAccount
    {
        public ElronAccount()
        {
            Transactions = new HashSet<ElronTransaction>();
            PeriodTickets = new HashSet<ElronPeriodTicket>();
        }

        public string Id { get; set; }
        public decimal? Balance { get; set; }
        public int? PeriodTicketThreshold { get; set; }
        public decimal? BalanceThreshold { get; set; }
        public DateTime LastCheck { get; set; }
        public virtual ICollection<ElronPeriodTicket> PeriodTickets { get; set; }
        public virtual ICollection<ElronTransaction> Transactions { get; set; }
    }

    public class ElronTransaction
    {   
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public decimal Sum { get; set; }

        public virtual ElronTicket Ticket { get; set; }
        public virtual ElronPeriodTicket PeriodTicket { get; set; }
    }

    public class ElronTicket
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public string Url { get; set; }
    }

    public class ElronPeriodTicket
    {
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        [NotMapped]
        public bool IsActive
        {
            get
            {
                return this.ValidTo > DateTime.Now;
            }
        }
    }
}