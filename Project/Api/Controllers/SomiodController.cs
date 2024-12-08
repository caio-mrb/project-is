using Api.Models;
using Api.RequestsDTO;
using System;
using System.Collections.Generic;
using System.Data;
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

        [HttpPost]
        [Route("")]
        public IHttpActionResult PostApplication([FromBody] Application request)
        {
            if (request == null || request.ResType != "application" || string.IsNullOrEmpty(request.ResType))
            {
                return BadRequest("Invalid request body.");
            }

            if (request.Name == null)
            {
                request.Name = _dbHandler.GenerateUniqueName("App");
            }
            else
            {
                string checkQuery = "SELECT COUNT(1) FROM dbo.applications WHERE name = @appName";
                var checkParameters = new List<SqlParameter>
                {
                new SqlParameter("@appName", request.Name)
                };

                int existingCount = _dbHandler.ExecuteQuery(checkQuery,checkParameters,reader =>
                (int)reader[0]
                ).FirstOrDefault();

                if (existingCount > 0)
                {
                    return BadRequest("An application with this name already exists.");
                }
            }

            if(request.CreationDatetime == DateTime.MinValue)
                request.CreationDatetime = DateTime.UtcNow;

            string query = "INSERT INTO dbo.applications (name, creation_datetime) VALUES (@appName, @creationDatetime)";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@appName", request.Name),
                new SqlParameter("@creationDatetime", request.CreationDatetime)
            };

            try
            {
                int rowsAffected = _dbHandler.ExecuteNonQuery(query, parameters);

                if (rowsAffected > 0)
                {
                    return Ok("Application created successfully.");
                }
                else
                {
                    return BadRequest("Failed to create application.");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


    }
}