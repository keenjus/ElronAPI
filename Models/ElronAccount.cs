using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace ElronAPI.Models
{
    public class ElronAccount
    {
        public ElronAccount()
        {
            Transactions = new HashSet<ElronTransaction>();
            PeriodTickets = new HashSet<ElronPeriodTicket>();
        }

        [Key]
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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public decimal Sum { get; set; }

        public virtual ElronTicket Ticket { get; set; }
        public virtual ElronPeriodTicket PeriodTicket { get; set; }
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