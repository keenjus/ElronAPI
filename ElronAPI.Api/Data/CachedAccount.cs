using System;
using System.ComponentModel.DataAnnotations;

namespace ElronAPI.Api.Data
{
    public partial class CachedAccount
    {
        [Key]
        public string Id { get; set; }
        public string Data { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}
