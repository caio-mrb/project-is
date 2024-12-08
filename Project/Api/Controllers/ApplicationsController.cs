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
    [RoutePrefix("api/somiod/{appName}")]
    public class ApplicationsController : ApiController
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
        public IHttpActionResult SomiodLocateHandler(string appName)
        {
            string somiodLocate = GetSomiodLocate();

            if (somiodLocate == null)
                return GetApplication(appName);

            switch (somiodLocate)
            {
                case "container":
                case "notification":
                case "record":
                    return GetAllNames(appName, somiodLocate);
                default:
                    return BadRequest("Invalid somiod-locate value");
            }
        }

        public IHttpActionResult GetApplication(string appName)
        {
            string query = "SELECT * FROM dbo.applications WHERE name = @appName";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@appName", appName)
            };

            List<Application> results = _dbHandler.ExecuteQuery(query, parameters, reader =>
                new Application
                {
                    Id = (int)reader["id"],
                    Name = (string)reader["name"],
                    CreationDatetime = (DateTime)reader["creation_datetime"]
                }
            );

            if (results.Any())
                return Ok(results.First());
            
            return NotFound();
            

        }

        public IHttpActionResult GetAllNames(string appName, string somiodLocate)
        {
            string query = GetCommandText(somiodLocate);
            if (string.IsNullOrEmpty(query)) return BadRequest("Invalid somiod-locate value");

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@appName", appName)
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
                case "container":
                    return @"
                    SELECT c.name
                    FROM dbo.containers c
                    JOIN dbo.applications a ON c.parent = a.id
                    WHERE a.name = @appName";
                case "notification":
                    return @"SELECT n.name
                    FROM dbo.notifications n
                    JOIN dbo.containers c ON n.parent = c.id
                    JOIN dbo.applications a ON c.parent = a.id
                    WHERE a.name = @appName";
                case "record":
                    return @"SELECT r.name 
                    FROM dbo.records r
                    JOIN dbo.containers c ON r.parent = c.id
                    JOIN dbo.applications a ON c.parent = a.id
                    WHERE a.name = @appName";
                default:
                    return null;
            }
        }

    }
}
