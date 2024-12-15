using Api.Controllers;
using System.Collections.Generic;

namespace Api.Models
{
    public class Record : ChildModel
    {
        public string Content { get; set; }

        public bool isEqualTo(Record other)
        {
            return (this.isEqualTo((ChildModel)other)
                && this.Content == other.Content);
        }

        public override string GetResType()
        {
            List<string> locateList = new List<string>(BaseController.AvailableSomiodLocates);

            return locateList[3];
        }
    }
}