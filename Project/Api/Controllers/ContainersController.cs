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
    [RoutePrefix("api/somiod/{appName}")]
    public class ContainersController : ApiController
    {
        string connectionString = Api.Properties.Settings.Default.ConnStr;

        [HttpGet]
        [Route("{contName}")]
        public IHttpActionResult GetContainer(string appName, string contName)
        {
            Container c = null;
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
                WHERE c.name = @contName AND a.name = @appName";
                command.Parameters.AddWithValue("@appName", appName);
                command.Parameters.AddWithValue("@contName", contName);
                command.CommandType = System.Data.CommandType.Text;
                command.Connection = connection;

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    c = new Container();
                    c.Id = (int)reader["id"];
                    c.Name = (string)reader["name"];
                    c.CreationDatetime = (DateTime)reader["creation_datetime"];
                    c.Parent = (int)reader["parent"];
                }
                reader.Close();
                return Ok(c);

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
