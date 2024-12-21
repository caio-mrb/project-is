using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLightApp.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDatetime { get; set; }
        public int Parent { get; set; }
        public int Event { get; set; }
        public string Endpoint { get; set; }
        public bool Enabled { get; set; }
    }
}
