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
    public class RecordsController : ChildController
    {
        [HttpGet]
        [Route("{recName}")]
        public IHttpActionResult GetRecord(string appName, string contName, string recName)
        {
            string query = @"
                            SELECT * 
                            FROM " + new Record().GetDatabase() + @" r
                            JOIN " + new Container().GetDatabase() + @" c ON r.parent = c.id
                            JOIN " + new Application().GetDatabase() + @" a ON c.parent = a.id
                            WHERE a.name = @appName AND c.name = @contName AND r.name = @recName";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@appName", appName),
                new SqlParameter("@contName", contName),
                new SqlParameter("@recName", recName)
            };

            return GetEntityHttpAnswer(query, parameters, reader =>
                new Record
                {
                    Id = (int)reader["id"],
                    Name = (string)reader["name"],
                    Content = (string)reader["content"],
                    CreationDatetime = (DateTime)reader["creation_datetime"],
                    Parent = (int)reader["parent"]
                }
            );
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult PostRecord(string appName, string contName, [FromBody] Record request)
        {
            var validationResult = ValidateRequest(request);
            if (validationResult != null) return validationResult;


            if (string.IsNullOrEmpty(request.Content))
                return BadRequest("Content field can't be empty.");

            var parentId = CheckIfExists(new Container {Name = contName});

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
                    return BadRequest("A record with this name already exists.");
            }

            if (request.CreationDatetime == DateTime.MinValue)
                request.CreationDatetime = DateTime.UtcNow;

            string insertQuery = @"
                                 INSERT INTO "+ request.GetDatabase() +@" (name, creation_datetime, parent, content)
                                 VALUES (@notiName, @creationDatetime, @parentId, @content)";
            var insertParameters = new List<SqlParameter>
            {
                new SqlParameter("@notiName", request.Name),
                new SqlParameter("@creationDatetime", request.CreationDatetime),
                new SqlParameter("@parentId", request.Parent),
                new SqlParameter("@content", request.Content)
            };

            return ExecuteWithMessage(insertQuery, insertParameters,
                "Record created successfully.",
                "Failed to create record.");
        }

    }
}
