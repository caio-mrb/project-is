using Api.Controllers;
using System.Collections.Generic;

namespace Api.Models
{
    public class Notification : ChildModel
    {
        public int Event { get; set; }

        public string Endpoint { get; set; }

        public bool? Enabled { get; set; }

        public bool isEqualTo(Notification other)
        {
            return (this.isEqualTo((ChildModel)other)
                && this.Event == other.Event
                && this.Endpoint == other.Endpoint
                && this.Enabled == other.Enabled);
        }

        public override string GetResType()
        {
            List<string> locateList = new List<string>(BaseController.AvailableSomiodLocates);

            return locateList[2];
        }
    }
}