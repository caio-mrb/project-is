using Api.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

namespace Api.Controllers
{
    [RoutePrefix("api/somiod/{appName}/{contName}")]
    public class ContainersController : ApiController
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
        public IHttpActionResult SomiodLocateHandler(string appName, string contName)
        {
            string somiodLocate = GetSomiodLocate();

            if (somiodLocate == null)
                return GetContainer(appName, contName);

            switch (somiodLocate)
            {
                case "notification":
                case "record":
                    return GetAllNames(appName, contName, somiodLocate);
                default:
                    return BadRequest("Invalid somiod-locate value");
            }
        }

        public IHttpActionResult GetContainer(string appName, string contName)
        {
            string query = @"
                            SELECT *
                            FROM dbo.containers c
                            JOIN dbo.applications a ON c.parent = a.id
                            WHERE a.name = @appName AND c.name = @contName";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@appName", appName),
                new SqlParameter("@contName", contName)
            };

            List<Container> results = _dbHandler.ExecuteQuery(query, parameters, reader =>
                new Container
                {
                    Id = (int)reader["id"],
                    Name = (string)reader["name"],
                    CreationDatetime = (DateTime)reader["creation_datetime"],
                    Parent = (int)reader["parent"]
                }
            );

            if (results.Any())
                return Ok(results.First());

            return NotFound();

        }

        public IHttpActionResult GetAllNames(string appName, string contName, string somiodLocate)
        {
            string query = GetCommandText(somiodLocate);
            if (string.IsNullOrEmpty(query)) return BadRequest("Invalid somiod-locate value");

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@appName", appName),
                new SqlParameter("@contName", contName)
            };

            List<string> results = _dbHandler.ExecuteQuery(query, parameters, reader =>
                reader["name"].ToString()
            );

            return Ok(results);
        }

        public string GetCommandText(string somiodLocate)
        {
            switch (somiodLocate)
            {
                case "notification":
                    return @"SELECT n.name
                    FROM dbo.notifications n
                    JOIN dbo.containers c ON n.parent = c.id
                    JOIN dbo.applications a ON c.parent = a.id
                    WHERE a.name = @appName AND c.name = @contName";
                case "record":
                    return @"SELECT r.name 
                    FROM dbo.records r
                    JOIN dbo.containers c ON r.parent = c.id
                    JOIN dbo.applications a ON c.parent = a.id
                    WHERE a.name = @appName AND c.name = @contName";
                default:
                    return null;
            }
        }
    }
}
