using System;

namespace Api.Models
{
    public class Container
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDatetime { get; set; }
        public int Parent { get; set; }
    }
}