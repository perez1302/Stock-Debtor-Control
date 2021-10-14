using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Demo.Models;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;

namespace Demo.Controllers
{
    public class DebtorsTransactionsController : Controller
    {
        private DemoDB db = new DemoDB();


        public ActionResult Unavailable()
        {
            
            return View();
        }

        public ActionResult Transactions(string Sorting_Order)
        {
            var debtorsTransactions = db.debtorsTransactions.Include(d => d.DebtorMaster).Include(d => d.StockMaster);
            // Getting details of which type of user is logged on
            var userID = (int)Session["UserID"];
            string idNo = "";
            int accCode = 0;
            foreach (var item in db.Users)
            {
                if (item.UserID == userID)
                {
                    idNo = item.IDNo;
                }
            }

            foreach (var item1 in db.DebtorMasters)
            {
                if (item1.IDNo.Equals(idNo))
                {
                    accCode = item1.AccCode;
                }
            }
            ViewBag.GrossValueSort = String.IsNullOrEmpty(Sorting_Order) ? "GrossValue" : "";
            ViewBag.TransactionDate = Sorting_Order == "TransactionDate" ? "Transaction_Date" : "TransactionDate";
            var DebtorsTransaction = from dm in db.debtorsTransactions select dm;
            //Sorting method(highest-lowest/lowest-highest)
            switch (Sorting_Order)
            {
                case "GrossValue":
                    DebtorsTransaction = DebtorsTransaction.OrderBy(dm => dm.GrossValue);
                    break;

                case "TransactionDate":
                    DebtorsTransaction = DebtorsTransaction.OrderBy(dm => dm.TransactionDate);
                    break;
                case "Transaction_Date":
                    DebtorsTransaction = DebtorsTransaction.OrderByDescending(dm => dm.TransactionDate);
                    break;

                default:
                    DebtorsTransaction = DebtorsTransaction.OrderByDescending(dm => dm.GrossValue);
                    break;
            }
           

            return View(DebtorsTransaction.Where(t => t.AccCode == accCode).ToList());
        }


      //View for confirmation of purchase for debtor
        public ActionResult Confirm()
        {
            var debtorsTransactions = db.debtorsTransactions.Include(d => d.DebtorMaster).Include(d => d.StockMaster);
            var userID = (int)Session["UserID"];
            string idNo = "";
            int accCode = 0;
            // Getting details of which type of user is logged on
            foreach (var item in db.Users)
            {
                if (item.UserID == userID)
                {
                    idNo = item.IDNo;
                }
            }

            foreach (var item1 in db.DebtorMasters)
            {
                if (item1.IDNo.Equals(idNo))
                {
                    accCode = item1.AccCode;
                }
            }


            return View(db.debtorsTransactions.Where(t => t.AccCode == accCode).ToList());
        }
        
        public ActionResult List(string Sorting_Order, string Search_Data) // viewing of all transactions by logged in debtor
        {

            //Sorting method(highest-lowest/lowest-highest)
            ViewBag.GrossValueSort = String.IsNullOrEmpty(Sorting_Order) ? "GrossValue" : "";
            ViewBag.TransactionDate = Sorting_Order == "TransactionDate" ? "Transaction_Date" : "TransactionDate";
            var DebtorsTransaction = from dm in db.debtorsTransactions select dm;
            {
                DebtorsTransaction = DebtorsTransaction.Where(dm => dm.TransactionID.ToString().ToUpper().Contains(Search_Data.ToUpper()));

            }
            switch (Sorting_Order)
            {
                case "GrossValue":
                    DebtorsTransaction = DebtorsTransaction.OrderBy(dm => dm.GrossValue);
                    break;

                case "TransactionDate":
                    DebtorsTransaction = DebtorsTransaction.OrderBy(dm => dm.TransactionDate);
                    break;
                case "Transaction_Date":
                    DebtorsTransaction = DebtorsTransaction.OrderByDescending(dm => dm.TransactionDate);
                    break;

                default:
                    DebtorsTransaction = DebtorsTransaction.OrderByDescending(dm => dm.GrossValue);
                    break;
            }
            var debtorsTransactions = db.debtorsTransactions.Include(d => d.DebtorMaster).Include(d => d.StockMaster);
            return View(DebtorsTransaction.ToList());
        }
        public ActionResult Index(string Sorting_Order) // view used for employees to view all debtors transactions
        {

            //Sorting method(highest-lowest/lowest-highest)
            ViewBag.GrossValueSort = String.IsNullOrEmpty(Sorting_Order) ? "GrossValue" : "";
           ViewBag.TransactionDate = Sorting_Order=="TransactionDate"? "Transaction_Date" : "TransactionDate";
            var DebtorsTransaction = from dm in db.debtorsTransactions select dm;
            
            switch (Sorting_Order)
            {
                case "GrossValue":
                    DebtorsTransaction = DebtorsTransaction.OrderBy(dm => dm.GrossValue);
                    break;

                case "TransactionDate":
                    DebtorsTransaction = DebtorsTransaction.OrderBy(dm => dm.TransactionDate);
                    break;
                case "Transaction_Date":
                    DebtorsTransaction = DebtorsTransaction.OrderByDescending(dm => dm.TransactionDate);
                    break;

                default:
                    DebtorsTransaction = DebtorsTransaction.OrderByDescending(dm => dm.GrossValue);
                    break;
            }
            var debtorsTransactions = db.debtorsTransactions.Include(d => d.DebtorMaster).Include(d => d.StockMaster);
            return View(DebtorsTransaction.ToList());
        }
        // GET: DebtorsTransactions/Details/5
        public ActionResult Details(int? id) // Used to create invoice
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DebtorsTransaction debtorsTransaction = db.debtorsTransactions.Find(id);
            if (debtorsTransaction == null)
            {
                return HttpNotFound();
            }


          //  int transID = 0;
            int InvoiceNo = 0;
          //  int accCode = 0;
            double totalExVat = 0;
           // int qty = 0;
            double vat = 0;
            double total = 0;
            string Total1 = "";
            //Creating List to store details to create PDF invoive
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[6] {
                            new DataColumn("TransactionID", typeof(int)),
                            new DataColumn("AccountCode", typeof(int)),
                             new DataColumn("Item", typeof(string)),
                            new DataColumn("Quantity", typeof(int)),
                            new DataColumn("TotalExcVat", typeof(string)),
                           
                            new DataColumn("Vat", typeof(string))});

            foreach (var item1 in db.debtorsTransactions)
            {
                if (item1.TransactionID == id)
                {
                    dt.Rows.Add(item1.TransactionID, item1.AccCode,item1.StockDes,item1.Qty,item1.GrossValue.ToString("R0.00"), item1.VatValue.ToString("R0.00"));
                    vat = item1.VatValue;//Storing vat value in another variable for displaying on the invoice
                    totalExVat = item1.GrossValue;//Storing amount exc vat value in another variable for displaying on the invoice
                    InvoiceNo = item1.TransactionID+1;//Using TransactionID and adding a number to it to get an Invoice number generated

                }
            }

     
            total = vat + totalExVat;
            Total1 = total.ToString();// to get a rand value saving the total as a string for displaying
            
            // the layout of the pdf
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {



                    StringBuilder sb = new StringBuilder();
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                    sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>INVOICE</b></td></tr>");
                    sb.Append("<tr><td colspan = '2'></td></tr>");
                    sb.Append("<tr><td><b>Invoice No: </b>");
                    sb.Append(InvoiceNo);
                    sb.Append("</td><td align = 'right'><b>Date: </b>");
                    sb.Append(DateTime.Now);
                    sb.Append(" </td></tr>");

                    
                    sb.Append("</table>");
                    sb.Append("<br />");

                    //Generate Invoice Items Grid.
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
                    //Generating the total
                    sb.Append("<tr><td align = 'right' colspan = '");
                    sb.Append(dt.Columns.Count - 1);
                    sb.Append("'>Total</td>");
                    sb.Append("<td>");
                    sb.Append(total.ToString("R0.00"));
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




            return View(debtorsTransaction);
        }

        
        public ActionResult Create()//Adding of transaction to database
        {
            ViewBag.AccCode = new SelectList(db.DebtorMasters, "AccCode", "DebtorName");
            ViewBag.StockCode = new SelectList(db.stockMasters, "StockCode", "StockDes");
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TransactionID,AccCode,StockCode,Qty")] DebtorsTransaction debtorsTransaction)//Adding of transaction to database
        {
            double vat = 0;
            double GrossAmount = 0;
            double SP = 0;
            double available = 0;
            // Getting details of which type of user is logged on
            var userID = (int)Session["UserID"];
            string idNo = "";
            int accCode = 0;
            foreach (var item in db.Users)
            {
                if (item.UserID == userID)
                {
                    idNo = item.IDNo;
                }
            }

            foreach (var item1 in db.DebtorMasters)
            {
                if (item1.IDNo.Equals(idNo))
                {
                    accCode = item1.AccCode;
                }
            }
      
            if (ModelState.IsValid)

            {
                string stockname;
                foreach (var item in db.stockMasters)
                {
                    if(item.StockCode==debtorsTransaction.StockCode)
                    {
                        stockname = item.StockDes;
                        SP = item.SellingPrice;                            // Storage of data to do validation
                        debtorsTransaction.StockDes = stockname;
                        available = item.StockAvailable;
                        break;
                    }
                }
                if (available > debtorsTransaction.Qty)  //Validating if stock is available
                {

                    GrossAmount = SP * debtorsTransaction.Qty;// calcuation of total before vat
                    vat = GrossAmount * 0.15;// calculation of vat
                    // adding value to database
                    debtorsTransaction.SellingPrice = SP;
                    debtorsTransaction.GrossValue = GrossAmount; 
                    debtorsTransaction.VatValue = vat;
                    debtorsTransaction.AccCode = accCode;

                    



                    debtorsTransaction.TransactionDate = DateTime.Now;
                    db.debtorsTransactions.Add(debtorsTransaction);
                    db.SaveChanges();
                    return RedirectToAction("Confirm");
                }
                else
                {
                    return RedirectToAction("Unavailable");
                }
            }

            ViewBag.AccCode = new SelectList(db.DebtorMasters, "AccCode", "DebtorName", debtorsTransaction.AccCode);
            ViewBag.StockCode = new SelectList(db.stockMasters, "StockCode", "StockDes", debtorsTransaction.StockCode,debtorsTransaction.StockDes);
            return View(debtorsTransaction);
        }

        // Used to update all necessary tables once order is confirmed
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DebtorsTransaction debtorsTransaction = db.debtorsTransactions.Find(id);
            if (debtorsTransaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccCode = new SelectList(db.DebtorMasters, "AccCode", "DebtorName", debtorsTransaction.AccCode);
            ViewBag.StockCode = new SelectList(db.stockMasters, "StockCode", "StockDes", debtorsTransaction.StockCode);
            return View(debtorsTransaction);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TransactionID,AccCode,StockCode,StockDes,SellingPrice,Qty,GrossValue,VatValue,TransactionDate")] DebtorsTransaction debtorsTransaction)
        {

            // using this method to do all the updates in different tables once order is confirmed. When the debtor reviews the order he/she will be redirected to this part and once purchased 
            //is clicked all tables will be updated accordingly
            if (ModelState.IsValid)
            {
                double amount = debtorsTransaction.GrossValue + debtorsTransaction.VatValue;
                int Stock = debtorsTransaction.StockCode;
                double cost = 0;
                double CP = 0;
                double avail = 0;

                foreach (var item3 in db.stockMasters)
                {
                    if (item3.StockCode == Stock)
                    {
                        cost = item3.Cost;
                       avail = item3.StockAvailable;
                       
                     
                    }
                }

                CP = cost * debtorsTransaction.Qty;// working out the new cost to date for debtor
               
                
                    foreach (var item2 in db.DebtorMasters)
                    {
                        if (item2.AccCode == debtorsTransaction.AccCode)
                        {
                            item2.Balance = item2.Balance + amount;
                            item2.CostToDate = item2.CostToDate + CP;
                            item2.SalesToDate = item2.SalesToDate + amount;
                        }
                    }


                    foreach(var item4 in db.stockMasters)
                    {
                        item4.QtySold = item4.QtySold + debtorsTransaction.Qty;
                        item4.StockAvailable = item4.StockAvailable - debtorsTransaction.Qty;
                        item4.TotalSales = item4.TotalSales + amount;
                        
                    }
                    

                    db.Entry(debtorsTransaction).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Transactions");
                
            
            }
            ViewBag.AccCode = new SelectList(db.DebtorMasters, "AccCode", "DebtorName", debtorsTransaction.AccCode);
            ViewBag.StockCode = new SelectList(db.stockMasters, "StockCode", "StockDes", debtorsTransaction.StockCode);
            return View(debtorsTransaction);


        }

        // remvoing a transaction only an employee allowed to use this option
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DebtorsTransaction debtorsTransaction = db.debtorsTransactions.Find(id);
            if (debtorsTransaction == null)
            {
                return HttpNotFound();
            }
            return View(debtorsTransaction);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)// Removal of transaction once debtor paid and is confirmed by employee. Only an employee has access to this
        {
            int transID = id;
            double amount = 0;
            int accCode = 0;
            DebtorsTransaction debtorsTransaction = db.debtorsTransactions.Find(id);
            foreach(var item in db.debtorsTransactions)
            {
                if(item.TransactionID==transID)
                {
                    accCode = item.AccCode;
                    amount = item.GrossValue + item.VatValue;
                }
            }

            foreach(var item1 in db.DebtorMasters)
            {
                if(item1.AccCode==accCode)
                {
                    item1.Balance = item1.Balance - amount;
                }
            }
            db.debtorsTransactions.Remove(debtorsTransaction);
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
