using AutoRental.Models;
using IronPdf;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutoRental.Controllers
{
    public class RentalController : Controller
    {
        AutoRentalDBEntities db = new AutoRentalDBEntities();
        // GET: Rental
        public ActionResult Index()
        {
            var result = (from r in db.Rentals
                          join c in db.Cars on r.CarNo equals c.CarNo
                          join p in db.People on r.IDPerson equals p.IDPerson
                          select new RentalView
                          {
                              IDRental = r.IDRental,
                              CarNo = r.CarNo,
                              IDPerson = r.IDPerson,
                              Name = p.Name,
                              Surname = p.Surname,
                              Price = r.Price,
                              StartDate = r.StartDate,
                              EndDate = r.EndDate,
                              Status = c.Status
                          }).ToList();
            return View(result);
        }
        [HttpGet]
        public ActionResult GetCar()
        {
            var car = db.Cars.ToList();
            return Json(car, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Getid(int id)
        {
            var person = (from s in db.People where s.IDPerson == id select s.Name).ToList();
            return Json(person, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Getstatus(string carno)
        {
            var carstatus = (from s in db.Cars where s.CarNo == carno select s.Status).FirstOrDefault();
            return Json(carstatus, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save(Rental rental)
        {
            if (ModelState.IsValid)
            {
                db.Rentals.Add(rental);
                var car = db.Cars.SingleOrDefault(e => e.CarNo == rental.CarNo);
                if (car == null)
                {
                    return HttpNotFound("404");
                }
                car.Status = "rental";
                db.Entry(car).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rental);
        }
        //[HttpPost]
        public ActionResult Download(int? id)
        {
            Rental rental = db.Rentals.Find(id);
            var renderer = new HtmlToPdf();
            string template = "<div style='font-size:30px;'><center><u>Umowa wynajmu samochodu</u></center></div>" +
                "               <div style='font-size:25px;'>Stronami niniejszej umowy są:" +
                "                <p style='font-size:23px;'>1." + rental.Name + "</p>" +
                "                <p style='font-size:23px;'>Nazywany dalej Wypożyczający</p>" +
                "                <p style='font-size:23px;'>2. ASP.NET project </p>" +
                "                <p style='font-size:23px;'>Nazywany dalej Pożyczający</p></div>" +
                "                <div><p style='font-size:23px;'>Przedmiotem umowy jest samochód o numerach: <b>" + rental.CarNo + "</b></p>" +
                "                <p style='font-size:23px;'>Udostępniony Wypożyczającemu w dniu: <b>" + rental.StartDate + "</b></p>" +
                "                <p style='font-size:23px;'>Za opłatą w wysokości: <b>" + rental.Price + " PLN </b> za każdy dzień.</p>" +
                "                <p style='font-size:23px;'>Umowa wygasa z dniem: <b>" + rental.EndDate + "</b></p></div>";
            string path = "C:/Document.pdf";
            var PDF = renderer.RenderHtmlAsPdf(template);
            PDF.SaveAs(path);
            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = path;
            process.Start();
            return RedirectToAction("Index");
        }

    }
}