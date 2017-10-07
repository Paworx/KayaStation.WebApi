using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KayaStation.API.Models
{
    public class Token
    {
        public string RequestToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresIn { get; set; }
    }
}
