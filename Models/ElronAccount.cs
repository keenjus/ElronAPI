using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElronAPI.Models
{
    public class ElronAccount
    {
        [Key]
        public string Id { get; set; }
        public decimal? Balance { get; set; }
        public DateTime LastCheck { get; set; }
    }
}