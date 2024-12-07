using System;

namespace Api.Models
{
    public class Container
    {
        public int Id { get; }
        public string Name { get; set; }
        public DateTime CreationDatetime { get; set; }
        public int Parent { get; }
    }
}