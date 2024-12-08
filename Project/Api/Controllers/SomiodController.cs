using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Api.Controllers
{
    [RoutePrefix("api/somiod")]
    public class SomiodController : ApiController
    {
        private readonly DatabaseHandler _dbHandler = new DatabaseHandler();

        private string GetSomiodLocate()
        {
            return Request.Headers.Contains("somiod-locate")
                ? Request.Headers.GetValues("somiod-locate").FirstOrDefault()
                : null;
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult SomiodLocateHandler()
        {
            string somiodLocate = GetSomiodLocate();

            switch (somiodLocate)
            {
                case "application":
                case "container":
                case "notification":
                case "record":
                    return GetAllNames(somiodLocate);
                default:
                    return BadRequest("Invalid somiod-locate value");
            }
        }

        public IHttpActionResult GetAllNames(string somiodLocate)
        {
            string query = GetCommandText(somiodLocate);
            if (string.IsNullOrEmpty(query)) return BadRequest("Invalid somiod-locate value");

            List<string> results = _dbHandler.ExecuteQuery(query, new List<SqlParameter>(), reader =>
                reader["name"].ToString()
            );

            return Ok(results);
        }

        public string GetCommandText(string somiodLocate)
        {
            switch (somiodLocate)
            {
                case "application":
                    return @"
                    SELECT name
                    FROM dbo.applications";
                case "container":
                    return @"
                    SELECT name
                    FROM dbo.containers";
                case "notification":
                    return @"SELECT name
                    FROM dbo.notifications";
                case "record":
                    return @"SELECT name 
                    FROM dbo.records r";
                default:
                    return null;
            }
        }

    }
}