using System;

namespace Api.Models
{
    public class Notification
    {
        public int Id { get; }
        public string Name { get; set; }
        public DateTime CreationDatetime { get; set; }
        public int ContainerId { get; }
        public char EventType { get; set; }
        public string EndPoint { get; set; }
        public bool Enabled { get; set; }
    }
}