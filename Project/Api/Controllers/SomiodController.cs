using Api.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Http;
using System.Data.Entity.Design.PluralizationServices;

namespace Api.Controllers
{
    [RoutePrefix("api/somiod")]
    public class SomiodController : ParentController
    {
        PluralizationService pluralizationService = PluralizationService.CreateService(new CultureInfo("en-US"));

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
            List<string> locateList = new List<string>(AvailableSomiodLocates);

            string database = null;

            if (somiodLocate == locateList[0])
                database = new Application().GetDatabase();

            if (somiodLocate == locateList[1])
                database = new Container().GetDatabase();

            if (somiodLocate == locateList[2])
                database = new Notification().GetDatabase();

            if (somiodLocate == locateList[3])
                database = new Record().GetDatabase();

            if(database == null)
                return null;

            return @"
                   SELECT name
                   FROM " + database;
        }

        #endregion

        #region Application

        [HttpGet]
        [Route("{appName}")]
        public IHttpActionResult SomiodLocateHandler(string appName)
        {
            if (CheckIfExists(new Application {Name = appName}) <= 0)
                return BadRequest("Unable to find this application.");

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

        public Application GetExistentApplication(string appName)
        {
            string query = @"
                           SELECT *
                           FROM " + new Application().GetDatabase() + @"
                           WHERE name = @appName";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@appName", appName)
            };

            return GetEntity(query, parameters, reader => new Application
                            {
                                Id = (int)reader["id"],
                                Name = (string)reader["name"],
                                CreationDatetime = (DateTime)reader["creation_datetime"]
                            });

        }

        public string GetApplicationCommandText(string somiodLocate)
        {
            List<string> locateList = new List<string>(AvailableSomiodLocates);

            if (somiodLocate == locateList[1])
                return @"
                       SELECT c.name
                       FROM " + new Container().GetDatabase() + @" c
                       JOIN " + new Application().GetDatabase() + @" a ON c.parent = a.id
                       WHERE a.name = @appName";

            if (somiodLocate == locateList[2])
                return @"SELECT n.name
                       FROM " + new Notification().GetDatabase() + @" n
                       JOIN " + new Container().GetDatabase() + @" c ON n.parent = c.id
                       JOIN " + new Application().GetDatabase() + @" a ON c.parent = a.id
                       WHERE a.name = @appName";

            if (somiodLocate == locateList[3])
                return @"SELECT r.name 
                       FROM " + new Record().GetDatabase() + @" r
                       JOIN " + new Container().GetDatabase() + @" c ON r.parent = c.id
                       JOIN " + new Application().GetDatabase() + @" a ON c.parent = a.id
                       WHERE a.name = @appName";
                
             return null;
            
        }

        public IHttpActionResult GetApplication(string appName)
        {
            Application application = GetExistentApplication(appName);

            if (application == null)
                return BadRequest("Unable to find this application");

            return Ok(application);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult PostApplication([FromBody] Application request)
        {
            var validationResult = ValidateRequest(request);
            if (validationResult != null)
                return validationResult;

            if (string.IsNullOrEmpty(request.Name))
            {
                request.Name = GenerateUniqueName(request);
            }
            else
            {
                if (CheckIfExists(request) > 0)
                    return BadRequest("An application with this name already exists.");
            }

            if (request.CreationDatetime == DateTime.MinValue)
                request.CreationDatetime = DateTime.UtcNow;

            string insertQuery = @"
                                 INSERT INTO " + request.GetDatabase() + @" (name, creation_datetime)
                                 VALUES (@appName, @creationDatetime)";
            var insertParameters = new List<SqlParameter>
            {
                new SqlParameter("@appName", request.Name),
                new SqlParameter("@creationDatetime", request.CreationDatetime)
            };

            return ExecuteWithMessage(insertQuery, insertParameters,
                "Application created successfully.",
                "Failed to create application.");
        }

        [HttpPatch]
        [Route("{appName}")]
        public IHttpActionResult PatchApplication(string appName, [FromBody] Application request)
        {
            var validationResult = ValidateRequest(request);
            if (validationResult != null) return validationResult;

            Application oldApp = GetExistentApplication(appName);

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
                if (CheckIfExists(request) > 0)
                    return BadRequest("An application with this name already exists.");
            }

            if (request.CreationDatetime == DateTime.MinValue)
                request.CreationDatetime = oldApp.CreationDatetime;

            if (request.isEqualTo(oldApp))
                return BadRequest("Nothing to update in this application.");

            string insertQuery = @"
                                 UPDATE " + request.GetDatabase() + @"
                                 SET name = @appName, creation_datetime = @creationDatetime
                                 WHERE id = @oldAppId";
            var insertParameters = new List<SqlParameter>
            {
                new SqlParameter("@appName", request.Name),
                new SqlParameter("@creationDatetime", request.CreationDatetime),
                new SqlParameter("@oldAppId", oldApp.Id)
            };

            return ExecuteWithMessage(insertQuery, insertParameters,
                "Application updated successfully.",
                "Failed to update application.");
        }

        [HttpDelete]
        [Route("{appName}")]
        public IHttpActionResult DeleteApplication(string appName)
        {
            if (CheckIfExists(new Application {Name = appName}) <= 0)
                return BadRequest("Unable to find this application.");

            string deleteQuery = @"
                           DELETE
                           FROM " + new Application().GetDatabase() + @"
                           WHERE name = @appName";
            var deleteParameters = new List<SqlParameter>
            {
                new SqlParameter("@appName", appName)
            };

            return ExecuteWithMessage(deleteQuery, deleteParameters,
                "Application deleted successfully.",
                "Failed to delete application.");
        }

        #endregion

        #region Container

        [HttpGet]
        [Route("{appName}/{contName}")]
        public IHttpActionResult SomiodLocateHandler(string appName, string contName)
        {
            (int parentId,Container container) = GetExistentContainer(appName, contName);

            if (parentId <= 0)
                return BadRequest("Unable to find this application.");

            if (container == null) 
                return BadRequest("Unable to find this container.");
            
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

        public static (int, Container) GetExistentContainer(string appName, string contName)
        {
            var parentId = CheckIfExists(new Application { Name = appName });

            if (parentId <= 0)
                return (0,null);

            string query = @"
                            SELECT *
                            FROM " + new Container().GetDatabase() + @" c
                            JOIN " + new Application().GetDatabase() + @" a ON c.parent = a.id
                            WHERE a.name = @appName AND c.name = @contName";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@appName", appName),
                new SqlParameter("@contName", contName)
            };

            Container container = GetEntity(query, parameters, reader => new Container
                                            {
                                                Id = (int)reader["id"],
                                                Name = (string)reader["name"],
                                                CreationDatetime = (DateTime)reader["creation_datetime"],
                                                Parent = (int)reader["parent"]
                                            });

            return (parentId, container);

        }

        public IHttpActionResult GetContainer(string appName, string contName)
        {
            (_, Container container) = GetExistentContainer(appName,contName);

            if (container == null)
                return BadRequest("Unable to find this container.");

            return Ok(container);
        }

        public string GetContainerCommandText(string somiodLocate)
        {
            List<string> locateList = new List<string>(AvailableSomiodLocates);

            if(somiodLocate == locateList[2])
                    return @"SELECT n.name
                    FROM " + new Notification().GetDatabase() + @" n
                    JOIN " + new Container().GetDatabase() + @" c ON n.parent = c.id
                    JOIN " + new Application().GetDatabase() + @" a ON c.parent = a.id
                    WHERE a.name = @appName AND c.name = @contName";

            if (somiodLocate == locateList[3])
                return @"SELECT r.name 
                    FROM " + new Record().GetDatabase() + @" r
                    JOIN " + new Container().GetDatabase() + @" c ON r.parent = c.id
                    JOIN " + new Application().GetDatabase() + @" a ON c.parent = a.id
                    WHERE a.name = @appName AND c.name = @contName";

            return null;

        }

        [HttpPost]
        [Route("{appName}")]
        public IHttpActionResult PostContainer(string appName, [FromBody] Container request)
        {
            var validationResult = ValidateRequest(request);
            if (validationResult != null) return validationResult;

            var parentId = CheckIfExists(new Application {Name = appName});

            if (parentId <= 0)
                return BadRequest("Unable to find this application.");

            if (request.Parent == 0)
                request.Parent = parentId;

            if (request.Parent != parentId)
                return BadRequest("Mismatch application id.");

            if (string.IsNullOrEmpty(request.Name))
            {
                request.Name = GenerateUniqueName(request);
            }
            else
            {
                if (CheckIfExists(request) > 0)
                    return BadRequest("A container with this name already exists.");
            }

            if (request.CreationDatetime == DateTime.MinValue)
                request.CreationDatetime = DateTime.UtcNow;

            string insertQuery = @"
                                 INSERT INTO " + request.GetDatabase() + @" (name, creation_datetime, parent)
                                 VALUES (@contName, @creationDatetime, @parentId)";
            var insertParameters = new List<SqlParameter>
            {
                new SqlParameter("@contName", request.Name),
                new SqlParameter("@creationDatetime", request.CreationDatetime),
                new SqlParameter("@parentId", request.Parent)
            };

            return ExecuteWithMessage(insertQuery, insertParameters,
                "Container created successfully.",
                "Failed to create container.");
        }

        [HttpPatch]
        [Route("{appName}/{contName}")]
        public IHttpActionResult PatchContainer(string appName, string contName, [FromBody] Container request)
        {
            var validationResult = ValidateRequest(request);
            if (validationResult != null) return validationResult;

            (int parentId,Container oldCont) = GetExistentContainer(appName, contName);

            if (parentId <= 0)
                return BadRequest("Unable to find this application.");

            if (request.Parent == 0)
                request.Parent = parentId;

            if(CheckIfExists(new Application { Id = request.Parent }) <= 0)
                return BadRequest("Unable to find new parent application.");

            if (oldCont == null)
                return BadRequest("Unable to find this container.");

            if (request.Id <= 0)
                request.Id = oldCont.Id;

            if (request.Id != oldCont.Id)
                return BadRequest("Invalid container id.");

            if (string.IsNullOrEmpty(request.Name))
                request.Name = oldCont.Name;

            if (request.Name != oldCont.Name)
            {
                if (CheckIfExists(request) > 0)
                    return BadRequest("An container with this name already exists.");
            }

            if (request.CreationDatetime == DateTime.MinValue)
                request.CreationDatetime = oldCont.CreationDatetime;

            if (request.isEqualTo(oldCont))
                return BadRequest("Nothing to update in this container.");

            string insertQuery = @"
                                 UPDATE " + request.GetDatabase() + @"
                                 SET name = @contName, creation_datetime = @creationDatetime, parent = @parent
                                 WHERE id = @oldContId";
            var insertParameters = new List<SqlParameter>
            {
                new SqlParameter("@contName", request.Name),
                new SqlParameter("@creationDatetime", request.CreationDatetime),
                new SqlParameter("@oldContId", oldCont.Id),
                new SqlParameter("@parent", request.Parent)
            };

            return ExecuteWithMessage(insertQuery, insertParameters,
                "Container updated successfully.",
                "Failed to update container.");
        }


        [HttpDelete]
        [Route("{appName}/{contName}")]
        public IHttpActionResult DeleteContainer(string appName, string contName)
        {
            (int parentId, Container container) = GetExistentContainer(appName, contName);

            if(parentId <= 0)
                return BadRequest("Unable to find this application.");

            if (container == null)
                return BadRequest("Unable to find this container.");

            string deleteQuery = @"
                           DELETE
                           FROM " + new Container().GetDatabase() + @"
                           WHERE name = @contName";
            var deleteParameters = new List<SqlParameter>
            {
                new SqlParameter("@contName", contName)
            };

            return ExecuteWithMessage(deleteQuery, deleteParameters,
                "Container deleted successfully.",
                "Failed to delete container.");
        }
        #endregion

    }
}