﻿using Api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    [RoutePrefix("api/somiod/applications")]
    public class ApplicationsController : ApiController
    {
        string connectionString = Api.Properties.Settings.Default.ConnStr;

        [Route("")]
        public IEnumerable<Application> GetAllApplications()
        {
            List<Application> applications = new List<Application>();

            SqlConnection connection = new SqlConnection(connectionString);

            const string queryString = "SELECT * FROM dbo.applications";

            try
            {
                connection.Open();
                Console.WriteLine("Connection opened successfully.");

                SqlCommand command = new SqlCommand(queryString, connection);

                SqlDataReader reader = command.ExecuteReader();
                Console.WriteLine("Query executed successfully.");

                while (reader.Read())
                {
                    Console.WriteLine("Reading a record...");
                    Application application = new Application
                    {
                        Id = (int)reader["id"],
                        Name = (string)reader["name"],
                        CreationDatetime = (DateTime)reader["creation_datetime"]
                    };
                    applications.Add(application);
                }

                Console.WriteLine($"Total applications found: {applications.Count}");
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
                Console.WriteLine(ex.Message);
            }

            return applications;

        }
        /*public IHttpActionResult GetApplication(int id)
        {
            var product = products.FirstOrDefault((p) => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product); //Respecting HTTP errors (200 OK)
        }

        // GET: Applications/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Applications/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Applications/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Applications/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Applications/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Applications/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }*/
    }
}
