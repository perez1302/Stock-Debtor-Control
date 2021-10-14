using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Demo.Models;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;

namespace Demo.Controllers
{
    public class StockTransactionFilesController : Controller
    {
        private DemoDB db = new DemoDB();


        public ActionResult Details1(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockTransactionFile stockTransactionFile = db.stockTransactionFiles.Find(id);
            if (stockTransactionFile == null)
            {
                return HttpNotFound();
            }

            
        
       

                
            



                return View(stockTransactionFile);
        }

        
        public ActionResult Index()// Used for employee to review order placed before confirming
        {
            var stockTransactionFiles = db.stockTransactionFiles.Include(s => s.StockMaster);
            return View(stockTransactionFiles.ToList());
        }
        public ActionResult List(string Sorting_Order) // used for employee to view all stock transactions
        {
            var stockTransactionFiles = db.stockTransactionFiles.Include(s => s.StockMaster);
            ViewBag.Total = String.IsNullOrEmpty(Sorting_Order) ? "Total" : "";
            ViewBag.StockTransactionDate = Sorting_Order == "StockTransactionDate" ? "StockTransaction_Date" : "StockTransactionDate";
            var stocktransaction = from dm in db.stockTransactionFiles select dm;
            //Sorting method(highest-lowest/lowest-highest)
            switch (Sorting_Order)
            {
                case "Total":
                    stocktransaction = stocktransaction.OrderBy(dm => dm.Total);
                    break;

                case "StockTransactionDate":
                    stocktransaction = stocktransaction.OrderBy(dm => dm.StockTransactionDate);
                    break;
                case "StockTransaction_Date":
                    stocktransaction = stocktransaction.OrderByDescending(dm => dm.StockTransactionDate);
                    break;

                default:
                   stocktransaction = stocktransaction.OrderByDescending(dm => dm.StockTransactionDate);
                    break;
            }

            return View(stocktransaction.ToList());
        }

        
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockTransactionFile stockTransactionFile = db.stockTransactionFiles.Find(id);
            if (stockTransactionFile == null)
            {
                return HttpNotFound();
            }
            return View(stockTransactionFile);
        }

        // Adding stock transaction to database
        public ActionResult Create()
        {
            ViewBag.StockCode = new SelectList(db.stockMasters, "StockCode", "StockDes");
            return View();
        }

        // Adding stock transaction to database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockTransactionID,StockCode,Quantity,UnitCost")] StockTransactionFile stockTransactionFile)
        {

            double mark = 0;
            double sp1 = 0;
            double amount1 = 0;
            double amount2 = 0;
            double total = 0;
            if (ModelState.IsValid)

            {

                string stockname;
                foreach (var item in db.stockMasters)
                {
                    if (item.StockCode == stockTransactionFile.StockCode)
                    {
                        stockname = item.StockDes;
                        mark = item.MarkUp;
                        stockTransactionFile.StockDes = stockname;
                        break;
                    }
                }
                sp1 = mark / 100;// working out the markup percentage
                amount1 = stockTransactionFile.UnitCost * sp1;// Calculating how much mark up is
                amount2 = stockTransactionFile.UnitCost + amount1;// add the mark up to the unit cost value
                stockTransactionFile.UnitSP = amount2;
                total = stockTransactionFile.UnitCost * stockTransactionFile.Quantity;
                stockTransactionFile.Total = total;
                stockTransactionFile.StockTransactionDate = DateTime.Now;
                db.stockTransactionFiles.Add(stockTransactionFile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StockCode = new SelectList(db.stockMasters, "StockCode", "StockDes", stockTransactionFile.StockCode);
            return View(stockTransactionFile);
        }

     
        public ActionResult Edit(int? id)// using this method to do updates in stock master table once transaction is done
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockTransactionFile stockTransactionFile = db.stockTransactionFiles.Find(id);
            if (stockTransactionFile == null)
            {
                return HttpNotFound();
            }
            ViewBag.StockCode = new SelectList(db.stockMasters, "StockCode", "StockDes", stockTransactionFile.StockCode);
            return View(stockTransactionFile);
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StockTransactionID,StockCode,StockDes,Quantity,UnitCost,UnitSP,Total,StockTransactionDate")] StockTransactionFile stockTransactionFile)
        {
            // using this method to do updates in stock master table once transaction is done
            if (ModelState.IsValid)


            {

                foreach(var item in db.stockMasters)
                   
                {
                    if (item.StockCode == stockTransactionFile.StockCode)
                    {
                        //Updating the main stock file after the transaction is done
                        item.Cost = stockTransactionFile.UnitCost;
                        item.SellingPrice = stockTransactionFile.UnitSP;
                        item.StockAvailable = item.StockAvailable + stockTransactionFile.Quantity;
                        item.QtyPurchased = item.QtyPurchased + stockTransactionFile.Quantity;
                        item.TotalPurchase = item.TotalPurchase + stockTransactionFile.Total;
                    }
                }
                db.Entry(stockTransactionFile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "StockMasters");
            }
            ViewBag.StockCode = new SelectList(db.stockMasters, "StockCode", "StockDes", stockTransactionFile.StockCode);
            return View(stockTransactionFile);
        }

       
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockTransactionFile stockTransactionFile = db.stockTransactionFiles.Find(id);
            if (stockTransactionFile == null)
            {
                return HttpNotFound();
            }
            return View(stockTransactionFile);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StockTransactionFile stockTransactionFile = db.stockTransactionFiles.Find(id);
            db.stockTransactionFiles.Remove(stockTransactionFile);
            db.SaveChanges();
            return RedirectToAction("Create");
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
