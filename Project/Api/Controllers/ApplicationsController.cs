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
    [RoutePrefix("api/somiod")]
    public class ApplicationsController : ApiController
    {
        string connectionString = Api.Properties.Settings.Default.ConnStr;

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllApplications()
        {
            string somiodLocate = Request.Headers.Contains("somiod-locate")
            ? Request.Headers.GetValues("somiod-locate").FirstOrDefault()
            : null;

            if (somiodLocate == "application")
            {
                List<Application> applications = new List<Application>();
                SqlConnection connection = null;

                try
                {
                    connection = new SqlConnection(connectionString);
                    connection.Open();

                    SqlCommand command = new SqlCommand();
                    command.CommandText = "SELECT * FROM dbo.applications";
                    command.CommandType = System.Data.CommandType.Text;
                    command.Connection = connection;

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Application application = new Application
                        {
                            Id = (int)reader["id"],
                            Name = (string)reader["name"],
                            CreationDatetime = (DateTime)reader["creation_datetime"]
                        };
                        applications.Add(application);
                    }

                    reader.Close();
                    connection.Close();
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

                return Ok(applications);
            }
            return BadRequest("Invalid somiod-locate value");
        }

        [HttpGet]
        [Route("{appName}")]
        public IHttpActionResult SomiodLocateHandler(string appName)
        {

            string somiodLocate = Request.Headers.Contains("somiod-locate")
                ? Request.Headers.GetValues("somiod-locate").FirstOrDefault()
                : null;

            if (somiodLocate == "application")
                return GetApplication(appName);

            if (somiodLocate == "container")
                return GetAllContainers(appName);

            if (somiodLocate == "notification")
                return GetAllNotifications(appName);

            if (somiodLocate == "record")
                return GetAllRecords(appName);

            return BadRequest("Invalid somiod-locate value");
        }

        public IHttpActionResult GetApplication(string appName)
        {
            Application a = null;
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.CommandText = "SELECT * FROM dbo.applications WHERE name = @appName";
                command.Parameters.AddWithValue("@appName", appName);
                command.CommandType = System.Data.CommandType.Text;
                command.Connection = connection;

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    a = new Application();
                    a.Id = (int)reader["id"];
                    a.Name = (string)reader["name"];
                    a.CreationDatetime = (DateTime)reader["creation_datetime"];
                }
                reader.Close();
                return Ok(a);

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

        public IHttpActionResult GetAllContainers(string appName)
        {
            List<Container> containers = new List<Container>();
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.CommandText = @"
                SELECT *
                FROM dbo.containers c
                JOIN dbo.applications a ON c.parent = a.id
                WHERE a.name = @appName";
                command.Parameters.AddWithValue("@appName", appName);
                command.CommandType = System.Data.CommandType.Text;
                command.Connection = connection;

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Container container = new Container
                    {
                        Id = (int)reader["id"],
                        Name = (string)reader["name"],
                        CreationDatetime = (DateTime)reader["creation_datetime"],
                        Parent = (int)reader["parent"]
                    };
                    containers.Add(container);
                }

                reader.Close();
                connection.Close();
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

            return Ok(containers);
        }

        public IHttpActionResult GetAllNotifications(string appName)
        {
            List<Notification> notifications = new List<Notification>();
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
                    WHERE a.name = @appName";
                command.Parameters.AddWithValue("@appName", appName);
                command.CommandType = System.Data.CommandType.Text;
                command.Connection = connection;

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Notification notification = new Notification
                    {
                        Id = (int)reader["id"],
                        Name = (string)reader["name"],
                        CreationDatetime = (DateTime)reader["creation_datetime"],
                        Parent = (int)reader["parent"],
                        Event = (int)reader["event"],
                        Endpoint = (string)reader["endpoint"],
                        Enabled = (bool)reader["enabled"]
                    };
                    notifications.Add(notification);
                }

                reader.Close();
                connection.Close();
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

            return Ok(notifications);

        }

        public IHttpActionResult GetAllRecords(string appName)
        {
            List<Record> records = new List<Record>();
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.CommandText = @"
                    SELECT *
                    FROM dbo.records r
                    JOIN dbo.containers c ON r.parent = c.id
                    JOIN dbo.applications a ON c.parent = a.id
                    WHERE a.name = @appName";
                command.Parameters.AddWithValue("@appName", appName);
                command.CommandType = System.Data.CommandType.Text;
                command.Connection = connection;

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Record record = new Record
                    {
                        Id = (int)reader["id"],
                        Name = (string)reader["name"],
                        Content = (string)reader["content"],
                        CreationDatetime = (DateTime)reader["creation_datetime"],
                        Parent = (int)reader["parent"]
                    };
                    records.Add(record);
                }

                reader.Close();
                connection.Close();
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

            return Ok(records);
        }

    }
}
