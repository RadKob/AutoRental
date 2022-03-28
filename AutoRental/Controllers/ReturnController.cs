using AutoRental.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.SqlServer;
using System.Data.Entity;
using System.Net.Mail;
using System.Net;

namespace AutoRental.Controllers
{
    public class ReturnController : Controller
    {
        AutoRentalDBEntities db = new AutoRentalDBEntities();
        // GET: Return
        public ActionResult Index()
        {
            return View(db.Returns.ToList());
        }
        public ActionResult Save(Return ret)
        {
            if (ModelState.IsValid)
            {
                db.Returns.Add(ret);
                var car = db.Cars.SingleOrDefault(e => e.CarNo == ret.CarNo);
                if(car == null)
                {
                    return HttpNotFound("404");
                }
                car.Status = "available";
                db.Entry(car).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ret);
        }
        [HttpPost]
        public ActionResult Getid(string carno)
        {
            var carn = (from s in db.Rentals join p in db.People on s.IDPerson equals p.IDPerson
                        where s.CarNo == carno
                        select new
                        { 
                            CarNo = s.CarNo,
                            IDPerson = s.IDPerson,
                            EndDate = s.EndDate,
                            Email = p.Email,
                            elsp = SqlFunctions.DateDiff("day", s.EndDate, DateTime.Now)
                        }).ToArray();
            return Json(carn,JsonRequestBehavior.AllowGet);
        }
    }
}