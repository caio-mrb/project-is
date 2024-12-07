using Microsoft.Analytics.Interfaces;
using Microsoft.Analytics.Types.Sql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Api.Models
{
    public class Containers
    {
        public int Id { get; }
        public string Name { get; set; }
        public DateTime CreationDatetime { get; set; }
        public int ApplicationId { get; }
    }
}