using Api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using static System.Net.Mime.MediaTypeNames;

namespace Api.Controllers
{
    [RoutePrefix("api/somiod")]
    public class SomiodController : ParentController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult SomiodLocateHandler()
        {
            string somiodLocate = GetSomiodLocate();

            return HandleSomiodLocate(
                somiodLocate,
                defaultAction: () => BadRequest("Invalid somiod-locate value"),
                getAllNamesAction: (appName, contName, locate) => GetAllNamesBase(locate, new List<SqlParameter>())
            );
        }

        protected override string GetCommandText(string somiodLocate)
        {
            string database = somiodLocate + "s";

            return @"
                    SELECT name
                    FROM dbo." + database;
        }

    }
}