using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AutoRental.Models;
using IronPdf;
using Newtonsoft.Json;

namespace AutoRental.Controllers
{
    public class CarController : Controller
    {
        private AutoRentalDBEntities db = new AutoRentalDBEntities();
        public ActionResult Index(string search, string type)
        {
            if(search == null)
            {
                return View(db.Cars.ToList());
            }
            else
            {
                switch (type)
                {
                    case "Brand":
                        var Brand = db.Cars.Where(x => x.TheBrand.StartsWith(search));
                        return View(Brand.ToList());
                    case "Model":
                        var Model = db.Cars.Where(x => x.TheModel.StartsWith(search));
                        return View(Model.ToList());
                    default:
                        return View(db.Cars.ToList());
                }
            }
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDCar,CarNo,TheBrand,TheModel,Year,Mileage,Status")] Car car)
        {
            if (ModelState.IsValid)
            {
                db.Cars.Add(car);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(car);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDCar,CarNo,TheBrand,TheModel,Year,Mileage,Engine,MaxPower,ABS,Doors,Mass,Color,Status")] Car car)
        {
            if (ModelState.IsValid)
            {
                db.Entry(car).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(car);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Car car = db.Cars.Find(id);
            db.Cars.Remove(car);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult CreatePDF(int? id)
        {
            Car car = db.Cars.Find(id);
            var renderer = new HtmlToPdf();
            string template = "<div style='background-color:lightgrey; font-size:30px;'><center>" + "<b>CarNo: </b>" + car.CarNo + "</cetner></div>" +
                "<div><div style='width:50%; float:left; font-size:26px;'><center><b>Brand:</b></br>" + car.TheBrand + "</center></div>" +
                "<div style='width:50%; float:left; font-size:26px;'><center><b>Model:</b></br>" + car.TheModel + "</center></div></div>" +
                "<hr><div><div style='width:50%; float:left; font-size:23px;'><center><b>Year of production:</b></br> " + car.Year+"</center></div>" +
                "<div style='width:50%; float:left; font-size:23px;'><center><b>Mileage: </b></br>" + car.Mileage + "</center></div></div>" +
                "<div><div style='width:50%; float:left; font-size:23px;'><center><b>Engine: </b></br> " + car.Engine + "</center></div>" +
                "<div style='width:50%; float:left; font-size:23px;'><center><b>Power[KM]: </b></br>" + car.MaxPower + "</center></div></div>" +
                "<hr><div><div style='background-color:lightgrey; font-size:30px;'><center><b>Additives</b></center></div><div style='width:50%; float:left; font-size:23px;'><center><b>Abs: </b></br> " + car.ABS + "</center></div>" +
                "<div style='width:50%; float:left; font-size:23px;'><center><b>Doors: </b></br>" + car.Doors + "</center></div></div>" +
                "<div style='width:50%; float:left; font-size:23px;'><center><b>Mass[kg]: </b></br> " + car.Mass + "</center></div>" +
                "<div style='width:50%; float:left; font-size:23px;'><center><b>Color: </b></br>" + car.Color + "</center></div></div>" +
                "<div style='width:100%; font-size:23px;'><center>" + car.Status + "</center></div>";
            string path = "C:/CarDetails.pdf";
            var PDF = renderer.RenderHtmlAsPdf(template);
            PDF.SaveAs(path);
            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = path;
            process.Start();
            return RedirectToAction("Index");
        }
        public ActionResult CreateJSON(Car car)
        {
            var personlist = db.Cars.ToList(); 
            string jsondata = new JavaScriptSerializer().Serialize(personlist);
            string path = Server.MapPath("~/App_Data/");
            System.IO.File.WriteAllText(path + "MyCars.json", jsondata);
            TempData["msg"] = "Json file Generated! check this in your App_Data folder";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult uploadCarFromJson(HttpPostedFileBase filejson)
        {
            if (!filejson.FileName.EndsWith(".json"))
            {
                ViewBag.errmsg = "Only JSON file";
            }
            else
            {
                filejson.SaveAs(Server.MapPath("~/empfolder/" + Path.GetFileName(filejson.FileName)));
                StreamReader reader = new StreamReader(Server.MapPath("~/empfolder/" + Path.GetFileName(filejson.FileName)));
                string jsondata = reader.ReadToEnd();
                List<Car> emptlist = JsonConvert.DeserializeObject<List<Car>>(jsondata);
                emptlist.ForEach(p =>
                {
                    Car car = new Car()
                    {
                        CarNo = p.CarNo,
                        TheBrand = p.TheBrand,
                        TheModel = p.TheModel,
                        Year = p.Year,
                        Mileage = p.Mileage,
                        Status = p.Status
                    };
                    db.Cars.Add(car);
                    db.SaveChanges();
                });
            }
            return RedirectToAction("Index");
        }
    }
}
