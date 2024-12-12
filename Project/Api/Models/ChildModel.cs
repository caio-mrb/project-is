﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Api.Models
{
    public class ChildModel : BaseModel
    {
        [XmlElement("Parent")]
        public int Parent { get; set; }

        public bool isEqualTo(ChildModel other)
        {
            return (this.isEqualTo((BaseModel)other)
                && this.Parent == other.Parent);
        }
    }
}