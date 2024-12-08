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
    public class NotificationsController : ApiController
    {
        private readonly DatabaseHandler _dbHandler = new DatabaseHandler();

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

            List<Notification> results = _dbHandler.ExecuteQuery(query, parameters, reader =>
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

            if (results.Any())
                return Ok(results.First());

            return NotFound();
        }

    }
}
