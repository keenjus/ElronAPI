using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElronAPI.Models
{
    public class ElronAccount
    {
        [Key]
        public string Number { get; set; }
        public decimal? Balance { get; set; }
    }
}