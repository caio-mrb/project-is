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
    [RoutePrefix("api/somiod/{appName}/{contName}")]
    public class NotificationsController : ApiController
    {
        string connectionString = Api.Properties.Settings.Default.ConnStr;

        [HttpGet]
        [Route("notification/{notiName}")]
        public IHttpActionResult GetNotification(string appName, string contName, string notiName)
        {
            Notification n = null;
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.CommandText = @"
                    SELECT *
                    FROM dbo.notifications n
                    JOIN dbo.containers c ON n.parent = c.id
                    JOIN dbo.applications a ON c.parent = a.id
                    WHERE a.name = @appName AND c.name = @contName AND n.name = @notiName";
                command.Parameters.AddWithValue("@appName", appName);
                command.Parameters.AddWithValue("@contName", contName);
                command.Parameters.AddWithValue("@notiName", notiName);
                command.CommandType = System.Data.CommandType.Text;
                command.Connection = connection;

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    n = new Notification();
                    n.Id = (int)reader["id"];
                    n.Name = (string)reader["name"];
                    n.CreationDatetime = (DateTime)reader["creation_datetime"];
                    n.Parent = (int)reader["parent"];
                    n.Event = (int)reader["event"];
                    n.Endpoint = (string)reader["endpoint"];
                    n.Enabled = (bool)reader["enabled"];
                }
                reader.Close();
                return Ok(n);

            }
            catch (Exception)
            {
                return NotFound();
            }
            finally
            {
                try
                {
                    connection.Close();
                }
                catch
                {

                }
            }
        }

    }
}
