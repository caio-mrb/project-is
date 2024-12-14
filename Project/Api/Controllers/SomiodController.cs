using Api.Models;
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
    public class SomiodController : ParentController
    {
        #region Somiod

        [HttpGet]
        [Route("")]
        public IHttpActionResult SomiodLocateHandler()
        {
            string somiodLocate = GetSomiodLocate();

            return HandleSomiodLocate(
                somiodLocate,
                defaultAction: () => BadRequest("Invalid somiod-locate value"),
                getAllNamesAction: (appName, contName, locate) => GetAllNamesBase(locate, new List<SqlParameter>(),
                                                                    GetCommandText: (commandLocate) => GetSomiodCommandText(somiodLocate))
            );
        }

        public string GetSomiodCommandText(string somiodLocate)
        {
            string database = somiodLocate + "s";

            return @"
                    SELECT name
                    FROM dbo." + database;
        }

        #endregion

        #region Application

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
                    }, GetCommandText: (commandLocate) => 
                    GetApplicationCommandText(somiodLocate)),
                appName: appName
            );
        }

        public string GetApplicationCommandText(string somiodLocate)
        {
            List<string> locateList = new List<string>(AvailableSomiodLocates);

            if(somiodLocate == locateList[1])
                return @"
                       SELECT c.name
                       FROM dbo.containers c
                       JOIN dbo.applications a ON c.parent = a.id
                       WHERE a.name = @appName";

            if (somiodLocate == locateList[2])
                return @"SELECT n.name
                       FROM dbo.notifications n
                       JOIN dbo.containers c ON n.parent = c.id
                       JOIN dbo.applications a ON c.parent = a.id
                       WHERE a.name = @appName";

            if (somiodLocate == locateList[3])
                return @"SELECT r.name 
                       FROM dbo.records r
                       JOIN dbo.containers c ON r.parent = c.id
                       JOIN dbo.applications a ON c.parent = a.id
                       WHERE a.name = @appName";
                
             return null;
            
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

            return GetEntityHttpAnswer(query, parameters, reader => new Application
            {
                Id = (int)reader["id"],
                Name = (string)reader["name"],
                CreationDatetime = (DateTime)reader["creation_datetime"]
            });
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult PostApplication([FromBody] Application request)
        {
            var validationResult = ValidateRequest(request);
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

        [HttpPatch]
        [Route("{appName}")]
        public IHttpActionResult PatchApplication(string appName, [FromBody] Application request)
        {
            var validationResult = ValidateRequest(request);
            if (validationResult != null) return validationResult;

            string queryOld = @"
                           SELECT *
                           FROM dbo.applications
                           WHERE name = @appName";
            var parametersOld = new List<SqlParameter>
            {
                new SqlParameter("@appName", appName)
            };

            Application oldApp = GetEntity(queryOld, parametersOld, reader => new Application
            {
                Id = (int)reader["id"],
                Name = (string)reader["name"],
                CreationDatetime = (DateTime)reader["creation_datetime"]
            });

            if (oldApp == null)
                return BadRequest("Unable to find this application.");

            if (request.Id <= 0)
                request.Id = oldApp.Id;

            if (request.Id != oldApp.Id)
                return BadRequest("Invalid application id.");

            if (string.IsNullOrEmpty(request.Name))
                request.Name = oldApp.Name;

            if (request.Name != oldApp.Name)
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
                request.CreationDatetime = oldApp.CreationDatetime;

            if (request.isEqualTo(oldApp))
                return BadRequest("Nothing to update in this application.");

            string insertQuery = @"
                                 UPDATE dbo.applications
                                 SET name = @appName, creation_datetime = @creationDatetime
                                 WHERE id = @oldAppId";
            var insertParameters = new List<SqlParameter>
            {
                new SqlParameter("@appName", request.Name),
                new SqlParameter("@creationDatetime", request.CreationDatetime),
                new SqlParameter("@oldAppId", oldApp.Id)
            };

            return ExecuteInsert(insertQuery, insertParameters,
                "Application updated successfully.",
                "Failed to update application.");
        }

        #endregion

        #region Container

        [HttpGet]
        [Route("{appName}/{contName}")]
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
                    }, GetCommandText: (commandLocate) =>
                        GetContainerCommandText(somiodLocate)),
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

            return GetEntityHttpAnswer(query, parameters, reader => new Container
            {
                Id = (int)reader["id"],
                Name = (string)reader["name"],
                CreationDatetime = (DateTime)reader["creation_datetime"],
                Parent = (int)reader["parent"]
            });
        }

        public string GetContainerCommandText(string somiodLocate)
        {
            List<string> locateList = new List<string>(AvailableSomiodLocates);

            if(somiodLocate == locateList[2])
                    return @"SELECT n.name
                    FROM dbo.notifications n
                    JOIN dbo.containers c ON n.parent = c.id
                    JOIN dbo.applications a ON c.parent = a.id
                    WHERE a.name = @appName AND c.name = @contName";

            if (somiodLocate == locateList[3])
                return @"SELECT r.name 
                    FROM dbo.records r
                    JOIN dbo.containers c ON r.parent = c.id
                    JOIN dbo.applications a ON c.parent = a.id
                    WHERE a.name = @appName AND c.name = @contName";

            return null;

        }

        [HttpPost]
        [Route("{appName}")]
        public IHttpActionResult PostContainer(string appName, [FromBody] Container request)
        {
            var validationResult = ValidateRequest(request);
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

        #endregion

    }
}