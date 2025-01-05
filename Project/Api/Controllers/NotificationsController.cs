using Api.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Http;
using System.Web.Management;
using System.Web.Routing;

namespace Api.Controllers
{
    [RoutePrefix("api/somiod/{appName}/{contName}/notification")]
    public class NotificationsController : ChildController
    {

        [HttpGet]
        [Route("{notiName}")]
        public IHttpActionResult GetNotification(string appName, string contName, string notiName)
        {

            (int parentId, int parentParentId, Notification notification) = GetExistentNotification(appName, contName, notiName);

            if(parentParentId <= 0)
                return BadRequest("Unable to find this application.");

            if(parentId <= 0)
                return BadRequest("Unable to find this container.");

            if (notification == null)
                return BadRequest("Unable to find this notification.");
            return Ok(notification);
        }

        public (int,int,Notification) GetExistentNotification(string appName, string contName, string notiName)
        {
            (int parentParentId, Container container) = SomiodController.GetExistentContainer(appName, contName);

            if (parentParentId == 0 || container == null)
                return (0, parentParentId, null);

            int parentId = container.Id;

            string query = @"
                            SELECT * 
                            FROM " + new Notification().GetDatabase() + @" n
                            JOIN " + new Container().GetDatabase() + @" c ON n.parent = c.id
                            JOIN " + new Application().GetDatabase() + @" a ON c.parent = a.id
                            WHERE a.name = @appName AND c.name = @contName AND n.name = @notiName";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@appName", appName),
                new SqlParameter("@contName", contName),
                new SqlParameter("@notiName", notiName)
            };

            Notification notification = ExecuteEntityOperation(query, parameters, reader =>
                new Notification
                {
                    Id = (int)reader["id"],
                    Name = (string)reader["name"],
                    CreationDatetime = (DateTime)reader["creation_datetime"],
                    Parent = (int)reader["parent"],
                    Event = (int)reader["event"],
                    Endpoint = (string)reader["endpoint"],
                    Enabled = (bool)reader["enabled"]
                }
            );

            return (parentId, parentParentId, notification);

        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult PostNotification(string appName, string contName, [FromBody] Notification request)
        {
            var validationResult = ValidateRequest(request);
            if (validationResult != null) return validationResult;

            if(request.Event != 1 && request.Event != 2)
                return BadRequest("Invalid event type. Must be 1(create) or 2(delete).");

            if(string.IsNullOrEmpty(request.Endpoint))
                return BadRequest("Endpoint field can't be empty.");

            if (!request.Enabled.HasValue)
                return BadRequest("Enabled field can't be empty. Must be True or False.");

            var parentId = CheckIfExists(new Container { Name = contName });

            if (parentId <= 0)
                return BadRequest("Unable to find this container.");

            if (request.Parent == 0)
                request.Parent = parentId;

            if (request.Parent != parentId)
                return BadRequest("Mismatch container id.");

            if (string.IsNullOrEmpty(request.Name))
            {
                request.Name = GenerateUniqueName(request);
            }
            else
            {
                if (CheckIfExists(request) > 0)
                    return BadRequest("A notification with this name already exists.");
            }

            if (request.CreationDatetime == DateTime.MinValue)
                request.CreationDatetime = DateTime.UtcNow;

            string insertQuery = @"
                                 INSERT INTO " + request.GetDatabase() + @" (name, creation_datetime, parent, event, endpoint, enabled)
                                 OUTPUT INSERTED.id, INSERTED.name, INSERTED.creation_datetime, INSERTED.parent, INSERTED.event, INSERTED.endpoint, INSERTED.enabled
                                 VALUES (@notiName, @creationDatetime, @parentId, @event, @endpoint, @enabled)";
            var insertParameters = new List<SqlParameter>
            {
                new SqlParameter("@notiName", request.Name),
                new SqlParameter("@creationDatetime", request.CreationDatetime),
                new SqlParameter("@parentId", request.Parent),
                new SqlParameter("@event", request.Event),
                new SqlParameter("@endpoint", request.Endpoint),
                new SqlParameter("@enabled", request.Enabled)
            };

            Notification notification = ExecuteEntityOperation(insertQuery, insertParameters, reader =>
                new Notification
                {
                    Id = (int)reader["id"],
                    Name = (string)reader["name"],
                    CreationDatetime = (DateTime)reader["creation_datetime"],
                    Parent = (int)reader["parent"],
                    Event = (int)reader["event"],
                    Endpoint = (string)reader["endpoint"],
                    Enabled = (bool)reader["enabled"]
                }
            );

            return Ok(notification);
        }


        [HttpDelete]
        [Route("{notiName}")]
        public IHttpActionResult DeleteNotification(string appName, string contName, string notiName)
        {
            (int parentId, int parentParentId, Notification notification) = GetExistentNotification(appName, contName, notiName);

            if (parentParentId <= 0)
                return BadRequest("Unable to find this application");

            if (parentId <= 0)
                return BadRequest("Unable to find this container");

            if (notification == null)
                return BadRequest("Unable to find this notification.");

            string deleteQuery = @"
                           DELETE
                           FROM " + new Notification().GetDatabase() + @"
                           WHERE name = @notiName";
            var deleteParameters = new List<SqlParameter>
            {
                new SqlParameter("@notiName", notiName)
            };

            return ExecuteNonQueryWithMessage(deleteQuery, deleteParameters,
                "Notification deleted successfully.",
                "Failed to delete notification.");
        }
    }
}
