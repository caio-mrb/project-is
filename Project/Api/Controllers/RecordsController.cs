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
    [RoutePrefix("api/somiod/{appName}/{contName}/record")]
    public class RecordsController : ApiController
    {
        private readonly DatabaseHandler _dbHandler = new DatabaseHandler();

        [HttpGet]
        [Route("{recName}")]
        public IHttpActionResult GetRecord(string appName, string contName, string recName)
        {
            string query = @"
                            SELECT * 
                            FROM dbo.records r
                            JOIN dbo.containers c ON r.parent = c.id
                            JOIN dbo.applications a ON c.parent = a.id
                            WHERE a.name = @appName AND c.name = @contName AND r.name = @recName";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@appName", appName),
                new SqlParameter("@contName", contName),
                new SqlParameter("@recName", recName)
            };

            List<Record> results = _dbHandler.ExecuteQuery(query, parameters, reader =>
                new Record
                {
                    Id = (int)reader["id"],
                    Name = (string)reader["name"],
                    Content = (string)reader["content"],
                    CreationDatetime = (DateTime)reader["creation_datetime"],
                    Parent = (int)reader["parent"]
                }
            );

            if (results.Any())
                return Ok(results.First());

            return NotFound();
        }

    }
}
