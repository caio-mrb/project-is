using System;

namespace Api.Models
{
    public abstract class BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDatetime { get; set; }


        public bool isEqualTo(BaseModel other)
        {
            return (this.Id == other.Id
                && this.Name == other.Name
                && this.CreationDatetime == other.CreationDatetime);
        }
    }
} 