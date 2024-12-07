using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Api.Controllers
{
    public class ContainersController : Controller
    {
        // GET: Containers
        public ActionResult Index()
        {
            return View();
        }

        // GET: Containers/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Containers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Containers/Create
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

        // GET: Containers/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Containers/Edit/5
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

        // GET: Containers/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Containers/Delete/5
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
        }
    }
}
