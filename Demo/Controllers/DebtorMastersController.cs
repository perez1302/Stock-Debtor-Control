using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Demo.Models;

namespace Demo.Controllers
{
    public class DebtorMastersController : Controller
    {
        private DemoDB db = new DemoDB();

      

        public ActionResult EmployeeAccess(string Sorting_Order)
        {
            ViewBag.BalanceSort = String.IsNullOrEmpty(Sorting_Order) ? "Balance" : "";
            ViewBag.SalesToDateSort = String.IsNullOrEmpty(Sorting_Order) ? "SalesToDate" : "";
            //Sorting method(highest-lowest/lowest-highest)
            var DebtorMasters = from dm in db.DebtorMasters select dm;
            switch (Sorting_Order)
            {
                case "Balance":
                    DebtorMasters = DebtorMasters.OrderBy(dm => dm.Balance);
                    break;

                case "SalesToDate":
                    DebtorMasters = DebtorMasters.OrderBy(dm => dm.SalesToDate);
                    break;

                default:
                    DebtorMasters = DebtorMasters.OrderByDescending(dm => dm.Balance);
                    break;
            }
            return View(DebtorMasters.ToList());
        }
        public ActionResult Index(string Sorting_Order)
        {
            // Getting details of which type of user is logged on
            var userID = (int)Session["UserID"];
            string idNo = "";

            foreach(var item in db.Users)
            {
                if(item.UserID==userID)
                {
                    idNo = item.IDNo;
                }
            }
            ViewBag.DebtorName = String.IsNullOrEmpty(Sorting_Order) ? "DebtorName" : "";
            ViewBag.DebtorSurname = String.IsNullOrEmpty(Sorting_Order) ? "DebtorSurname" : "";
            //Sorting method(highest-lowest/lowest-highest)
            var DebtorMasters = from dm in db.DebtorMasters select dm;
            switch (Sorting_Order)
            {
                case "DebtorName":
                    DebtorMasters = DebtorMasters.OrderBy(dm => dm.DebtorName);
                    break;

                case "DebtorSurname":
                    DebtorMasters = DebtorMasters.OrderBy(dm => dm.DebtorSurname);
                    break;

                default:
                    DebtorMasters = DebtorMasters.OrderByDescending(dm => dm.DebtorName);
                    break;
            }
            return View(DebtorMasters.Where(t => t.IDNo == idNo).ToList());
        }

        //Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DebtorMaster debtorMaster = db.DebtorMasters.Find(id);
            if (debtorMaster == null)
            {
                return HttpNotFound();
            }
            return View(debtorMaster);
        }

        //Adding of debtor to table
        public ActionResult Create()
        {
            return View();
        }

        //Adding of debtor to table

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AccCode,DebtorName,DebtorSurname,IDNo,DebtorsAddress,Suburb,Town,Province,CellNo,email")] DebtorMaster debtorMaster)
        {
           
           
            if (ModelState.IsValid)
            {
                db.DebtorMasters.Add(debtorMaster);
                db.SaveChanges();
                return RedirectToAction("EmployeeAccess");
            }

            return View(debtorMaster);
        }

        //Updating for debtor details
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DebtorMaster debtorMaster = db.DebtorMasters.Find(id);
            if (debtorMaster == null)
            {
                return HttpNotFound();
            }
            return View(debtorMaster);
        }

        //Updating for debtor details

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AccCode,DebtorName,DebtorSurname,IDNo,DebtorsAddress,Suburb,Town,Province,CellNo,email,Balance,SalesToDate,CostToDate")] DebtorMaster debtorMaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(debtorMaster).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("EmployeeAccess");
            }
            return View(debtorMaster);
        }

        // Deleting debtor
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DebtorMaster debtorMaster = db.DebtorMasters.Find(id);
            if (debtorMaster == null)
            {
                return HttpNotFound();
            }
            return View(debtorMaster);
        }

        // Deleting debtor
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DebtorMaster debtorMaster = db.DebtorMasters.Find(id);
            db.DebtorMasters.Remove(debtorMaster);
            db.SaveChanges();
            return RedirectToAction("EmployeeAccess");
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
