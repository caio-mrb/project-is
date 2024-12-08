using System;
using System.Xml.Serialization;

namespace Api.Models
{
    public abstract class BaseModel
    {
        [XmlElement("Id")]
        public int Id { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("CreationDatetime")]
        public DateTime CreationDatetime { get; set; }

        [XmlElement("ResType")]
        public string ResType { get; set; }
    }
}