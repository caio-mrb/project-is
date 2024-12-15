using System.Collections.Generic;
using Api.Controllers;

namespace Api.Models
{
    public class Application : BaseModel
    {
        public override string GetResType()
        {
            List<string> locateList = new List<string>(BaseController.AvailableSomiodLocates);

            return locateList[0];
        }
    }
}