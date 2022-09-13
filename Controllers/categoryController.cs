using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace vacrem.Controllers
{
    public class categoryController : Controller
    {  
        //
        // GET: /category/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /category/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /category/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /category/Create

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

        //
        // GET: /category/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /category/Edit/5

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

        //
        // GET: /category/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /category/Delete/5

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
