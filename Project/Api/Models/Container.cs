using System;
using System.Xml.Serialization;

namespace Api.Models
{
    [XmlRoot("Container")]
    public class Container : BaseModel
    {
        [XmlElement("Parent")]
        public int Parent { get; set; }
    }
}