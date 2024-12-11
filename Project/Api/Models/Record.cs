using System;
using System.Xml.Serialization;

namespace Api.Models
{
    public class Record : ChildModel
    {
        [XmlElement("Content")]
        public string Content { get; set; }
    }
}