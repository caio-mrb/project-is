using Api.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Api.Controllers
{
    public class BaseController : ApiController
    {
        protected readonly DatabaseHandler _dbHandler = new DatabaseHandler();

        protected IHttpActionResult GetEntity<T>(
            string query,
            List<SqlParameter> parameters,
            Func<SqlDataReader, T> mapEntity)
        {
            List<T> results = _dbHandler.ExecuteQuery(query, parameters, mapEntity);

            if (results.Any())
                return Ok(results.First());

            return NotFound();
        }

        public string GenerateUniqueName(string baseName, string table)
        {
            int counter = 1;

            while (true)
            {
                string candidateName = $"{baseName}{counter}";

                string checkQuery = "SELECT COUNT(1) FROM dbo." + table + " WHERE name = @name";
                var parameters = new List<SqlParameter>
            {
                new SqlParameter("@name", candidateName)
            };

                int existingCount = _dbHandler.ExecuteQuery(checkQuery, parameters, reader =>
                    (int)reader[0]
                ).FirstOrDefault();

                if (existingCount == 0)
                {
                    return candidateName;
                }

                counter++;
            }
        }

        protected IHttpActionResult ValidateRequest<T>(T request, string expectedResType) where T : BaseModel
        {
            if (request == null || request.ResType != expectedResType || string.IsNullOrEmpty(request.ResType))
            {
                return BadRequest("Invalid request body.");
            }
            return null;
        }

        protected int CheckIfExists(string query, List<SqlParameter> parameters)
        {
            return _dbHandler.ExecuteQuery(query, parameters, reader => (int)reader[0]).FirstOrDefault();
        }

        protected IHttpActionResult ExecuteInsert(string query, List<SqlParameter> parameters, string successMessage, string failureMessage)
        {
            try
            {
                int rowsAffected = _dbHandler.ExecuteNonQuery(query, parameters);

                if (rowsAffected > 0)
                {
                    return Ok(successMessage);
                }
                else
                {
                    return BadRequest(failureMessage);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}