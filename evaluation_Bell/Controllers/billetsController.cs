using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using evaluation_Bell.Models;
using System.Web.UI.DataVisualization.Charting;
using System.IO;
using System.Text;
using System.Drawing;


namespace evaluation_Bell.Controllers
{
    public class billetsController : Controller
    {
        private bellEvaluationDBEntities db = new bellEvaluationDBEntities();

        // GET: billets Index French Version
        public ActionResult Index(String option,String target)
        {
    
            var billet = db.billet.Include(b => b.departement).Include(b => b.employe);
            List<departement> departementList = db.departement.ToList();
            ViewBag.departementList = new SelectList(departementList, "idDepartement", "nomDepartement");

            if(!String.IsNullOrEmpty(target))
            {
                if (option == "projNom" )
                {
                    billet = billet.Where(b => b.nomProjet.Contains(target));
                }
                else if (option == "depName" )
                {
                    billet = billet.Where(b => b.departement.nomDepartement.Contains(target));
                }
                else if (option == "emploName" )
                {
                    billet = billet.Where(b => b.employe.nomEmploye.Contains(target));
                }
                else if (option == "description" )
                {
                    billet = billet.Where(b => b.description.Contains(target));
                }
            }
            return View(billet.ToList());

        }


        // GET: billets Index English Version
        public ActionResult IndexEng(String option, String target)
        {

            var billet = db.billet.Include(b => b.departement).Include(b => b.employe);
            List<departement> departementList = db.departement.ToList();
            ViewBag.departementList = new SelectList(departementList, "idDepartement", "nomDepartement");

            if (!String.IsNullOrEmpty(target))
            {
                if (option == "projNom")
                {
                    billet = billet.Where(b => b.nomProjet.Contains(target));
                }
                else if (option == "depName")
                {
                    billet = billet.Where(b => b.departement.nomDepartement.Contains(target));
                }
                else if (option == "emploName")
                {
                    billet = billet.Where(b => b.employe.nomEmploye.Contains(target));
                }
                else if (option == "description")
                {
                    billet = billet.Where(b => b.description.Contains(target));
                }
            }
            return View(billet.ToList());

        }
       

        // GET: billets/Create French Version
        public ActionResult Create()
        {
            List<departement> departementList = db.departement.ToList();
            ViewBag.departementList = new SelectList(departementList, "idDepartement", "nomDepartement");
            ViewBag.idDepartement = new SelectList(db.departement, "idDepartement", "nomDepartement");
            ViewBag.idEmploye = new SelectList(db.employe, "idEmploye", "nomEmploye");
            return View();
        }


        //this function used with jquery to set employees of each departement
        public JsonResult GetEmployeList(int idDepartement)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<employe> EmployeList = db.employe.Where(x => x.idDepartement == idDepartement).ToList();
            return Json(EmployeList, JsonRequestBehavior.AllowGet);
        }



        // POST: billets/Create French Version
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idBillet,nomProjet,description,dateCreation,idDepartement,idEmploye")] billet billet)
        {
            if (ModelState.IsValid)
            {
                db.billet.Add(billet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idDepartement = new SelectList(db.departement, "idDepartement", "nomDepartement", billet.idDepartement);
            ViewBag.idEmploye = new SelectList(db.employe, "idEmploye", "nomEmploye", billet.idEmploye);
            return View(billet);
        }




        // GET: billets/Create English Version
        public ActionResult CreateEng()
        {

            ViewBag.idDepartement = new SelectList(db.departement, "idDepartement", "nomDepartement");
            ViewBag.idEmploye = new SelectList(db.employe, "idEmploye", "nomEmploye");
            return View();
        }

        // POST: billets/Create English Version
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEng([Bind(Include = "idBillet,nomProjet,description,dateCreation,idDepartement,idEmploye")] billet billet)
        {
            if (ModelState.IsValid)
            {
                db.billet.Add(billet);
                db.SaveChanges();
                return RedirectToAction("IndexEng");
            }

            ViewBag.idDepartement = new SelectList(db.departement, "idDepartement", "nomDepartement", billet.idDepartement);
            ViewBag.idEmploye = new SelectList(db.employe, "idEmploye", "nomEmploye", billet.idEmploye);
            return View(billet);
        }


        //chart 
        public ActionResult Charte()
        {


            var billet = db.billet.GroupBy(x => x.nomProjet).Select(y => new { Element = y.Key, Counter = y.Count() }).ToList();
            List<departement> departementList = db.departement.ToList();
            ViewBag.departementList = new SelectList(departementList, "idDepartement", "nomDepartement");
            var chart = new Chart();
            var area = new ChartArea();
            chart.ChartAreas.Add(area);
            var series = new Series();
            foreach (var p in billet)
            {
                series.Points.AddXY(p.Element, p.Counter);
            }

            series.Label = "#PERCENT{P0}";
            series.Font = new Font("Arial", 8.0f, FontStyle.Bold);
            series.ChartType = SeriesChartType.Column;
            series["PieLabelStyle"] = "Outside";
            chart.Series.Add(series);
            var returnStream = new MemoryStream();
            chart.ImageType = ChartImageType.Png;
            chart.SaveImage(returnStream);
            returnStream.Position = 0;
            return new FileStreamResult(returnStream, "image/png");

        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
