using System;

namespace Api.Models
{
    public class Record : BaseModel
    {
        public string Content { get; set; }
        public int Parent { get; set; }
    }
}