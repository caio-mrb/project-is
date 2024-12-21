using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchApplication.Models
{
    public class ApiResponse
    {
        public bool Success { get; set; } 
        public string Message { get; set; }
        public string ResponseContent { get; set; }

    }
}
