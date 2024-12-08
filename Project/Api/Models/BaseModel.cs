using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public abstract class BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDatetime { get; set; }
    }
}