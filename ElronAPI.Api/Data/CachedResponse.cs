using System;
using System.ComponentModel.DataAnnotations;

namespace ElronAPI.Api.Data
{
    public partial class CachedResponse
    {
        [Key]
        public string Id { get; set; }
        public string Data { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}
