using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElronAPI.Api.Models
{
    public class JsonErrorResponseModel
    {
        public bool error { get; set; }
        public string message { get; set; }
    }
}
