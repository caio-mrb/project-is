using System;

namespace Api.Models
{
    public class Record
    {
        public int Id { get; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime CreationDatetime { get; set; }
        public int ContainerId { get; }
    }
}