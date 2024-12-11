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
    [RoutePrefix("api/somiod")]
    public class ApplicationsController : ParentController
    {

        [HttpGet]
        [Route("{appName}")]
        public IHttpActionResult SomiodLocateHandler(string appName)
        {
            string somiodLocate = GetSomiodLocate();

            return HandleSomiodLocate(
                somiodLocate,
                defaultAction: () => GetApplication(appName),
                getAllNamesAction: (name, contName, locate) =>
                    GetAllNamesBase(locate, new List<SqlParameter>
                    {
                        new SqlParameter("@appName", name)
                    }),
                appName: appName
            );
        }

        protected override string GetCommandText(string somiodLocate)
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

        public IHttpActionResult GetApplication(string appName)
        {
            string query = @"
                           SELECT *
                           FROM dbo.applications
                           WHERE name = @appName";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@appName", appName)
            };

            return GetEntity(query, parameters, reader => new Application
            {
                Id = (int)reader["id"],
                Name = (string)reader["name"],
                CreationDatetime = (DateTime)reader["creation_datetime"],
                ResType = "application"
            });
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult PostApplication([FromBody] Application request)
        {
            var validationResult = ValidateRequest(request, "application");
            if (validationResult != null) return validationResult;

            if (string.IsNullOrEmpty(request.Name))
            {
                request.Name = GenerateUniqueName("App", "applications");
            }
            else
            {
                string checkQuery = @"
                                    SELECT COUNT(1)
                                    FROM dbo.applications
                                    WHERE name = @appName";
                var checkParameters = new List<SqlParameter>
                {
                    new SqlParameter("@appName", request.Name)
                };

                if (CheckIfExists(checkQuery, checkParameters) > 0)
                    return BadRequest("An application with this name already exists.");
            }

            if (request.CreationDatetime == DateTime.MinValue)
                request.CreationDatetime = DateTime.UtcNow;

            string insertQuery = @"
                                 INSERT INTO dbo.applications (name, creation_datetime)
                                 VALUES (@appName, @creationDatetime)";
            var insertParameters = new List<SqlParameter>
            {
                new SqlParameter("@appName", request.Name),
                new SqlParameter("@creationDatetime", request.CreationDatetime)
            };

            return ExecuteInsert(insertQuery, insertParameters,
                "Application created successfully.",
                "Failed to create application.");
        }



        
    }
}
