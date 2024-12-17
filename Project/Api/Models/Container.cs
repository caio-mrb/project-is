using Api.Controllers;
using System.Collections.Generic;

namespace Api.Models
{
    public class Container : ChildModel
    {
        public override string GetResType()
        {
            List<string> locateList = new List<string>(BaseController.AvailableSomiodLocates);

            return locateList[1];
        }

    }
}