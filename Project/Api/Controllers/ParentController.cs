using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Api.Controllers
{
    public class ParentController : BaseController
    {
       
        protected string GetSomiodLocate()
        {
            return Request.Headers.Contains("somiod-locate")
                ? Request.Headers.GetValues("somiod-locate").FirstOrDefault()
                : null;
        }

        protected IHttpActionResult HandleSomiodLocate(
            string somiodLocate,
            Func<IHttpActionResult> defaultAction,
            Func<string, string, string, IHttpActionResult> getAllNamesAction,
            string appName = null,
            string contName = null)
        {
            if (somiodLocate == null)
                return defaultAction();

            if(IsValidSomiodLocate(somiodLocate))
                return getAllNamesAction(appName, contName, somiodLocate);
            
            return BadRequest("Invalid somiod-locate value");
            
        }

        protected IHttpActionResult GetAllNamesBase(
            string somiodLocate,
            List<SqlParameter> parameters,
            Func<string, string> GetCommandText)
        {
            string query = GetCommandText(somiodLocate);
            if (string.IsNullOrEmpty(query))
                return BadRequest("Invalid somiod-locate value");

            List<string> results = _dbHandler.ExecuteQuery(query, parameters, reader =>
                reader["name"].ToString()
            );

            return Ok(results);
        }
    }
}