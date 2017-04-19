using System;
using System.Collections.Generic;

namespace ElronAPI.Models
{
    public partial class Calendar
    {
        public int ServiceId { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
