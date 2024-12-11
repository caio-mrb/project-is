using System;

namespace Api.Models
{
    public class Notification : BaseModel
    {
        public int Parent { get; set; }
        public int Event { get; set; }
        public string Endpoint { get; set; }
        public bool? Enabled { get; set; }
    }
}