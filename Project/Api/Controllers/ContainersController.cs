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
    public class ContainersController : ParentController
    {

        [HttpGet]
        [Route("{contName}")]
        public IHttpActionResult SomiodLocateHandler(string appName, string contName)
        {
            string somiodLocate = GetSomiodLocate();

            return HandleSomiodLocate(
                somiodLocate,
                defaultAction: () => GetContainer(appName, contName),
                getAllNamesAction: (name, cont, locate) =>
                    GetAllNamesBase(locate, new List<SqlParameter>
                    {
                        new SqlParameter("@appName", name),
                        new SqlParameter("@contName", cont)
                    }),
                appName: appName,
                contName: contName
            );
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

            return GetEntity(query, parameters, reader => new Container
            {
                Id = (int)reader["id"],
                Name = (string)reader["name"],
                CreationDatetime = (DateTime)reader["creation_datetime"],
                Parent = (int)reader["parent"],
                ResType = "container"
            });
        }

        protected override string GetCommandText(string somiodLocate)
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

        [HttpPost]
        [Route("")]
        public IHttpActionResult PostContainer(string appName, [FromBody] Container request)
        {
            var validationResult = ValidateRequest(request, "container");
            if (validationResult != null) return validationResult;

            string parentQuery = @"
                                 SELECT id
                                 FROM dbo.applications
                                 WHERE name = @appName";
            var parentParameters = new List<SqlParameter>
            {
                new SqlParameter("@appName", appName)
            };

            var parentId = CheckIfExists(parentQuery, parentParameters);

            if (parentId <= 0)
                return BadRequest("Unable to find this application.");

            if (request.Parent == 0)
                request.Parent = parentId;

            if (request.Parent != parentId)
                return BadRequest("Mismatch application id.");

            if (string.IsNullOrEmpty(request.Name))
            {
                request.Name = GenerateUniqueName("Container", "containers");
            }
            else
            {
                string checkQuery = @"
                                    SELECT COUNT(1)
                                    FROM dbo.containers
                                    WHERE name = @contName";
                var checkParameters = new List<SqlParameter>
                {
                    new SqlParameter("@contName", request.Name)
                };

                if (CheckIfExists(checkQuery, checkParameters) > 0)
                    return BadRequest("A container with this name already exists.");
            }

            if (request.CreationDatetime == DateTime.MinValue)
                request.CreationDatetime = DateTime.UtcNow;

            string insertQuery = @"
                                 INSERT INTO dbo.containers (name, creation_datetime, parent)
                                 VALUES (@contName, @creationDatetime, @parentId)";
            var insertParameters = new List<SqlParameter>
            {
                new SqlParameter("@contName", request.Name),
                new SqlParameter("@creationDatetime", request.CreationDatetime),
                new SqlParameter("@parentId", request.Parent)
            };

            return ExecuteInsert(insertQuery, insertParameters,
                "Container created successfully.",
                "Failed to create container.");
        }
    }

}
