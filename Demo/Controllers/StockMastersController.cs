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
    public class StockMastersController : Controller
    {
        private DemoDB db = new DemoDB();

        //View sales and stock
        public ActionResult Index(string Sorting_Order)
        {
            //Sorting method(highest-lowest/lowest-highest)
            ViewBag.TotalSalesSort = String.IsNullOrEmpty(Sorting_Order) ? "TotalSales" : "";
          
            var StockMasters = from dm in db.stockMasters select dm;

            switch (Sorting_Order)
            {
                case "TotalSales":
                    StockMasters = StockMasters.OrderBy(dm => dm.TotalSales);
                    break;

               
                

                default:
                    StockMasters = StockMasters.OrderByDescending(dm => dm.TotalSales);
                    break;
            }
            return View(StockMasters.ToList());
        }
        public ActionResult DebtorView(string Sorting_Order) // used for the debtor to see what stock is available and price its selling for
        {
            //Sorting method(highest-lowest/lowest-highest)
            ViewBag.SellingPrice = String.IsNullOrEmpty(Sorting_Order) ? "SellingPrice" : "";

            var StockMasters = from dm in db.stockMasters select dm;

            switch (Sorting_Order)
            {
                case "SellingPrice":
                    StockMasters = StockMasters.OrderBy(dm => dm.SellingPrice);
                    break;




                default:
                    StockMasters = StockMasters.OrderByDescending(dm => dm.SellingPrice);
                    break;
            }
            return View(StockMasters.ToList());
        }


        //Used to generate invoice
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockMaster stockMaster = db.stockMasters.Find(id);
            if (stockMaster == null)
            {
                return HttpNotFound();
            }


            int InvoiceNo = 0;

            double vat1 = 0;
            double profits = 0;
            double qty = 0;
            double SP = 0;
            double total = 0;
            double total1 = 0;
            double total2 = 0;
            //Creating List to store details to create PDF invoive
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[7] {

                            new DataColumn("StockCode", typeof(int)),
                                new DataColumn("Stock", typeof(string)),
                             new DataColumn("QtySold", typeof(int)),
                            new DataColumn("UnitCost", typeof(string)),
                            new DataColumn("UnitSP", typeof(string)),
                            new DataColumn("VatPaid", typeof(string)),
                             new DataColumn("Profits", typeof(string))
                            });

            foreach (var item1 in db.stockMasters)
            {
                if (item1.StockCode == id)
                {
                    
                    total1 = item1.QtySold * item1.SellingPrice;//Calculating whats the total sales of the choosen item
                    vat1 = total1 * 0.15;//calculating vat value 
                    total2 = item1.Cost * item1.QtySold;
                    profits = total1-total2;//Calculating profits the company has made so far on choosen item

                    dt.Rows.Add(item1.StockCode,item1.StockDes, item1.QtySold, item1.Cost.ToString("R0.00"), item1.SellingPrice.ToString("R0.00"),vat1.ToString("R0.00"),profits.ToString("R0.00"));
                   
                    qty = item1.QtySold;
                    SP = item1.SellingPrice;

                    InvoiceNo = item1.StockCode + 1;//Using StockCode and adding a number to it to get an Invoice number generated

                }
            }
            total = qty * SP;

            // the layout of the pdf
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    StringBuilder sb = new StringBuilder();


                    sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                    sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>INVOICE DETAIL</b></td></tr>");
                    sb.Append("<tr><td colspan = '2'></td></tr>");
                    sb.Append("<tr><td><b>Invoice No: </b>");
                    sb.Append(InvoiceNo);
                    sb.Append("</td><td align = 'right'><b>Date: </b>");
                    sb.Append(DateTime.Now);
                    sb.Append(" </td></tr>");


                    sb.Append("</table>");
                    sb.Append("<br />");

                    //Generate Invoice (Bill) Items Grid.
                    sb.Append("<table border = '1'>");
                    sb.Append("<tr>");


                    foreach (DataColumn column in dt.Columns)
                    {
                        sb.Append("<th>");
                        sb.Append(column.ColumnName);
                        sb.Append("</th>");
                    }

                    sb.Append("</tr>");
                    foreach (DataRow row in dt.Rows)
                    {
                        sb.Append("<tr>");
                        foreach (DataColumn column in dt.Columns)
                        {
                            sb.Append("<td>");
                            sb.Append(row[column]);
                            sb.Append("</td>");
                        }
                        sb.Append("</tr>");
                    }

                    sb.Append("<tr><td align = 'right' colspan = '");
                    sb.Append(dt.Columns.Count - 1);
                    //Generating the total
                    sb.Append("'>Total</td>");
                    sb.Append("<td>");
                    sb.Append(total1.ToString("R0.00"));
                    sb.Append("</td>");
                    sb.Append("</tr></table>");


                    //Export HTML String as PDF.
                    StringReader sr = new StringReader(sb.ToString());
                    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();
                    htmlparser.Parse(sr);
                    pdfDoc.Close();

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=Invoice_" + InvoiceNo + ".pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.Write(pdfDoc);
                    Response.End();


                }



            }




                return View(stockMaster);
        }

        // Adding of new stock line to the database
        public ActionResult Create()
        {
            return View();
        }

        // Adding of new stock line to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockCode,StockDes,Cost,MarkUp,QtyPurchased")] StockMaster stockMaster)
        {

           double MUAmount = 0;
            double MarkUpAmount = 0;
            double SP = 0;
            if (ModelState.IsValid)
            {
                MUAmount = stockMaster.MarkUp / 100;
                MarkUpAmount = stockMaster.Cost*MUAmount;
                SP = stockMaster.Cost+MarkUpAmount;
                // When a item is added to stock the update for that stock in the main table is done here
                stockMaster.SellingPrice = SP; 
                stockMaster.TotalPurchase = stockMaster.Cost * stockMaster.QtyPurchased;
                stockMaster.StockAvailable = stockMaster.QtyPurchased;
                db.stockMasters.Add(stockMaster);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stockMaster);
        }

      
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockMaster stockMaster = db.stockMasters.Find(id);
            if (stockMaster == null)
            {
                return HttpNotFound();
            }
            return View(stockMaster);
        }

    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StockCode,StockDes,Cost,MarkUp,SellingPrice,QtyPurchased,TotalPurchase,QtySold,TotalSales,StockAvailable")] StockMaster stockMaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stockMaster).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stockMaster);
        }

        // Removing stock line
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockMaster stockMaster = db.stockMasters.Find(id);
            if (stockMaster == null)
            {
                return HttpNotFound();
            }
            return View(stockMaster);
        }

        // Removing stock line
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StockMaster stockMaster = db.stockMasters.Find(id);
            db.stockMasters.Remove(stockMaster);
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
    }
}
