﻿using Api.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Xml.Linq;

namespace Api.Controllers
{
    /// <summary>
    /// <c>BaseController</c> provides utility methods for API controllers, including database interactions, entity validation, 
    /// and common functionalities like unique name generation and request validation.
    /// </summary>
    public class BaseController : ApiController
    {
        /// <summary>
        /// A set of available Somiod locations. The order must not be changed to maintain backward compatibility.
        /// </summary>
        public static readonly HashSet<string> AvailableSomiodLocates = new HashSet<string>
        {
            // Don't edit the location order, cause will break existing code.
            // If you need to add a new location, insert it below the last one.
            "application",
            "container",
            "notification",
            "record"
        };

        /// <summary>
        /// Validates whether a given somiodLocate exists in the predefined set of available locations.
        /// </summary>
        /// <param name="somiodLocate">The location to validate.</param>
        /// <returns>True if valid; otherwise, false.</returns>
        public static bool IsValidSomiodLocate(string somiodLocate)
        {
            return AvailableSomiodLocates.Contains(somiodLocate);
        }

        /// <summary>
        /// Retrieves an entity from the database using the specified query and mapping function.
        /// </summary>
        /// <typeparam name="T">The type of entity to retrieve.</typeparam>
        /// <param name="query">The SQL query string.</param>
        /// <param name="parameters">List of SQL parameters to include in the query.</param>
        /// <param name="mapEntity">A function to map the SqlDataReader to the entity type.</param>
        /// <returns>The first entity found, or null if none exists.</returns>
        protected static T ExecuteEntityOperation<T>(
            string query,
            List<SqlParameter> parameters,
            Func<SqlDataReader, T> mapEntity)
        {
            List<T> results = DatabaseHandler.ExecuteQuery(query, parameters, mapEntity);

            return results.FirstOrDefault();
        }

        /// <summary>
        /// Generates a unique name for an entity by appending a counter to the base name.
        /// </summary>
        /// <typeparam name="T">The type of the request model.</typeparam>
        /// <param name="entity">The entity to use as template.</param>
        /// <returns>A unique name not found in the specified table.</returns>
        public string GenerateUniqueName<T>(T entity) where T : BaseModel
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            int counter = 1;

            while (true)
            {
                
                string candidateName = $"{textInfo.ToTitleCase(entity.GetResType())}{counter}";

                entity.Name = candidateName;

                if (CheckIfExists(entity) == 0)
                {
                    return candidateName;
                }

                counter++;
            }
        }

        /// <summary>
        /// Validates the request model to ensure it is not null.
        /// </summary>
        /// <typeparam name="T">The type of the request model.</typeparam>
        /// <param name="request">The request model to validate.</param>
        /// <returns>A BadRequest result if invalid, or null if valid.</returns>
        protected IHttpActionResult ValidateRequest<T>(T request) where T : BaseModel
        {
            if (request == null)
            {
                return BadRequest("Invalid request body.");
            }
            return null;
        }

        /// <summary>
        /// Checks if an entity exists in the database and returns its ID.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity to check.</param>
        /// <returns>The ID of the entity if it exists; otherwise, 0.</returns>
        protected static int CheckIfExists<T>(T entity) where T : BaseModel
        {
            bool hasEntityName = !string.IsNullOrEmpty(entity.Name);

            string checkQuery = @"
                                SELECT id
                                FROM " + entity.GetDatabase() + @"
                                WHERE " + (hasEntityName ? "name = @name OR " : "") + @"id = @id";
            var checkParameters = new List<SqlParameter>
            {
                new SqlParameter("@id", entity.Id)
            };
            if (hasEntityName)
                checkParameters.Add(new SqlParameter("@name", entity.Name));

            return DatabaseHandler.ExecuteQuery(checkQuery, checkParameters, reader => (int)reader[0]).FirstOrDefault();
        }

        /// <summary>
        /// Executes a query and returns a success or failure message based on the number of rows affected.
        /// </summary>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">List of SQL parameters to include in the query.</param>
        /// <param name="successMessage">The message to return if the query succeeds.</param>
        /// <param name="failureMessage">The message to return if the query fails.</param>
        /// <returns>An HTTP response with the success or failure message.</returns>
        protected IHttpActionResult ExecuteNonQueryWithMessage(
            string query,
            List<SqlParameter> parameters,
            string successMessage,
            string failureMessage)
        {
            try
            {
                int rowsAffected = DatabaseHandler.ExecuteNonQuery(query, parameters);

                if (rowsAffected > 0)
                {
                    // If results exist, return them with a success message
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
