using System;
using System.Xml.Serialization;

namespace Api.Models
{
    public class Notification : ChildModel
    {
        [XmlElement("Event")]
        public int Event { get; set; }

        [XmlElement("Endpoint")]
        public string Endpoint { get; set; }

        [XmlElement("Enabled")]
        public bool? Enabled { get; set; }
    }
}