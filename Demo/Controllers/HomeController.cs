using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Demo.Models;
using System.Security.Cryptography;
namespace ProjectMVC5.Controllers
{

    // this controller is used to handle the login and registration
    public class HomeController : Controller
    {
        private DemoDB db = new DemoDB();

        public ActionResult Successful()


        {
            return View();
        }
        public ActionResult DetailsIncorrect()


        { 
           return View();
        }
        //Registration done here

        public ActionResult Register()
        {
            return View();
        }

        //Registration done here
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User _user)
        {
            if (ModelState.IsValid)

            {


                string ID = _user.IDNo;
                bool check1 = false;
                string name = "";                                   // Getting all details to do validation
                string surname = "";
                string email = "";
                foreach(var item in db.DebtorMasters)
                {
                    if(item.IDNo==ID)
                    {
                        name = item.DebtorName;
                        surname = item.DebtorSurname;
                        email = item.email;
                        check1 = true;
                    }
                    else
                    {
                        check1 = false;
                    }
                }
                var check = db.Users.FirstOrDefault(s => s.Email == _user.Email);

                //Validation getting done if details are correct addition to the table will be done here
                if (check == null)
                {
                    if (_user.AccType.Equals("Debtor"))
                    {

                        if (check1 == true&&name==_user.FirstName&&surname==_user.LastName&&email==_user.Email)
                        {


                            _user.Password = GetMD5(_user.Password);
                            db.Configuration.ValidateOnSaveEnabled = false;
                            db.Users.Add(_user);
                            db.SaveChanges();
                            return RedirectToAction("Successful");
                        }
                        else
                        {

                            return RedirectToAction("DetailsIncorrect");// if details incorrect redirection will happen
                        }
                    }
                    else
                    {
                        _user.Password = GetMD5(_user.Password);
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.Users.Add(_user);
                        db.SaveChanges();
                        return RedirectToAction("Successful");

                    }
                }
                else
                {
                    ViewBag.error = "Email already exists or ID number doesnt exist";
                    return View();
                }


            }
            return View();


        }

        public ActionResult Login()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {

            if (ModelState.IsValid)
            {


                var f_password = GetMD5(password);
                var data = db.Users.Where(s => s.Email.Equals(email) && s.Password.Equals(f_password)).ToList();
                if (data.Count() > 0)
                {
                    // This takes the user details and add him to a session as logged on so only the specific views for which ever account type is logged on will show
                    Session["FullName"] = data.FirstOrDefault().FirstName + " " + data.FirstOrDefault().LastName;
                    Session["Email"] = data.FirstOrDefault().Email;
                    Session["UserID"] = data.FirstOrDefault().UserID;
                    if (data.FirstOrDefault().AccType.Equals("Debtor"))
                    {
                        return RedirectToAction("Index","DebtorMasters");
                    }
                    else
                    {
                        return RedirectToAction("Index","StockMasters");
                    }
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("Login");
                }
            }
            return View();
        }


        //Logout
        public ActionResult Logout()
        {
            Session.Clear();//removes session and logs oerson out
            return RedirectToAction("Login");
        }



        //create a string MD5 for hidden and encrypted passwords
        public static string GetMD5(string str)
        {
            //used to gain the password since its hidden upon entry
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }

    }
}