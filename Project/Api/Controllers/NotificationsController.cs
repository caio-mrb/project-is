using Api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;

namespace Api.Controllers
{
    [RoutePrefix("api/somiod/{appName}/{contName}/notification")]
    public class NotificationsController : ChildController
    {

        [HttpGet]
        [Route("{notiName}")]
        public IHttpActionResult GetNotification(string appName, string contName, string notiName)
        {
            string query = @"
                            SELECT * 
                            FROM dbo.notifications n
                            JOIN dbo.containers c ON n.parent = c.id
                            JOIN dbo.applications a ON c.parent = a.id
                            WHERE a.name = @appName AND c.name = @contName AND n.name = @notiName";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@appName", appName),
                new SqlParameter("@contName", contName),
                new SqlParameter("@notiName", notiName)
            };

            return GetEntityHttpAnswer(query, parameters, reader =>
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
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult PostNotification(string appName, string contName, [FromBody] Api.Models.Notification request)
        {
            var validationResult = ValidateRequest(request);
            if (validationResult != null) return validationResult;

            if(request.Event != 1 && request.Event != 2)
                return BadRequest("Invalid event type. Must be 1(create) or 2(delete).");

            if(string.IsNullOrEmpty(request.Endpoint))
                return BadRequest("Endpoint field can't be empty.");

            if (!request.Enabled.HasValue)
                return BadRequest("Enabled field can't be empty. Must be True or False.");

            string parentQuery = @"
                                 SELECT id
                                 FROM dbo.containers
                                 WHERE name = @contName";
            var parentParameters = new List<SqlParameter>
            {
                new SqlParameter("@contName", contName)
            };

            var parentId = CheckIfExists(parentQuery, parentParameters);

            if (parentId <= 0)
                return BadRequest("Unable to find this container.");

            if (request.Parent == 0)
                request.Parent = parentId;

            if (request.Parent != parentId)
                return BadRequest("Mismatch container id.");

            if (string.IsNullOrEmpty(request.Name))
            {
                request.Name = GenerateUniqueName("Notification", "notifications");
            }
            else
            {
                string checkQuery = @"
                                    SELECT COUNT(1)
                                    FROM dbo.notifications
                                    WHERE name = @notiName";
                var checkParameters = new List<SqlParameter>
                {
                    new SqlParameter("@notiName", request.Name)
                };

                if (CheckIfExists(checkQuery, checkParameters) > 0)
                    return BadRequest("A notification with this name already exists.");
            }

            if (request.CreationDatetime == DateTime.MinValue)
                request.CreationDatetime = DateTime.UtcNow;

            string insertQuery = @"
                                 INSERT INTO dbo.notifications (name, creation_datetime, parent, event, endpoint, enabled)
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

            return ExecuteInsert(insertQuery, insertParameters,
                "Notification created successfully.",
                "Failed to create notification.");
        }

    }
}
