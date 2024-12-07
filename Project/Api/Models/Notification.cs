using System;

namespace Api.Models
{
    public class Notification
    {
        public int Id { get; }
        public string Name { get; set; }
        public DateTime CreationDatetime { get; set; }
        public int Parent { get; }
        public int Event { get; set; }
        public string EndPoint { get; set; }
        public bool Enabled { get; set; }
    }
}