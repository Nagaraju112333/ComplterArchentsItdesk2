using ArchentsIT.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using static System.Net.WebRequestMethods;
using System.Runtime.Remoting.Services;
namespace ArchentsIT.Controllers
{
    // original

    public class UserRegisterController : Controller
    {
        // GET: UserRegister
        ArchnetsITHelpDesk db=new ArchnetsITHelpDesk();  
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult UserRegister()
        {
            return View();
        }
        [HttpPost]
       
        public ActionResult UserRegister(UserRegister user)
        {
            try
            {
                bool Status = false;
                string message = "";
                //b
                // Model Validation 

                #region //Email is already Exist 
                var isExist = IsEmailExist(user.Email);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    ViewBag.IsEmailExist = "Email Is Already Exist";
                    // return View(user);
                }
                else
                {
                    user.Password = Guid.NewGuid().ToString();
                    //TempData["name"] = user.Password;
                    #endregion
                    #region Generate Activation Code 
                    //user.ActivationCode = Guid.NewGuid();
                    // user.EmplID = Guid.NewGuid();
                    #endregion
                    #region  Password Hashing 
                    user.Password = Crypto.Hash(user.Password);
                    TempData["name"] = user.Password;
                    //   user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword); //
                    #endregion
                    // user.IsEmailVerified = false;
                    #region Save to Database
                    Status = true;
                    using (ArchnetsITHelpDesk dc = new ArchnetsITHelpDesk())
                    {
                        user.registercount = 1;
                        user.RoleType = 2;
                        dc.UserRegisters.Add(user);
                        dc.SaveChanges();

                        TempData["FirsttimeRegister"] = user.registercount;
                        //Send Email to User
                        TempData["Employeename"] = user.FirstName;
                        SendVerificationLinkEmail(user.Email);
                        message = "Registration successfully done. password " +
                            " has been sent to your email id:" + user.Email;
                        Status = true;
                        TempData["message"] = message;
                        TempData["Status"] = Status;
                        /* ViewBag.Message = message;
                         ViewBag.Status = Status;*/
                        return RedirectToAction("Login", "UserRegister");
                    }
                }
                #endregion


                /* else
                 {
                     message = "Invalid Request";
                 }*/
                ViewBag.Message = message;
                ViewBag.Status = Status;
                return View(user);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (ArchnetsITHelpDesk dc = new ArchnetsITHelpDesk())
            {
                var v = dc.UserRegisters.Where(a => a.Email == emailID).FirstOrDefault();
                return v != null;
            }
        }
        [NonAction]
        public void SendVerificationLinkEmail(string emailID)
        {
           // var verifyUrl = "/Account/" +"/" ;
           // var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var fromEmail = new MailAddress("Complaints.archents@outlook.com", emailID);
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "V@11@pu6eddy$"; // Replace with actual password
            string subject = "";
            string body = "";
            subject = "Your account is successfully created!";
            body = " <br/><br/>Hello <b>" + TempData["Employeename"] +"</b> <br/> You have been invited for IT support protal for any it related issue please login using following password <br/><b> Your Password is:</b>" +" "+""+" "
                +TempData["name"] + "" + "</a> ";
            MailMessage mc = new MailMessage("Complaints.archents@outlook.com", emailID);
            mc.Subject = subject;
            mc.Body = body;
            mc.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
            smtp.Timeout = 1000000;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            NetworkCredential nc = new NetworkCredential("Complaints.archents@outlook.com", "V@11@pu6eddy$");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = nc;
            smtp.Send(mc);
        }
        // Login View
        [HttpGet]
        public ActionResult Login()
        {
                return View();
        }
        // Logout Employee
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "UserRegister");
        }
        // Login Functionality
        [HttpPost]
        /*[ValidateAntiForgeryToken]*/
        public ActionResult Login(UserLogin user)
       {
           
                string message = "";
                /* var result = db.UserRegisters.Where(a => a.Email == user.EmailID && a.Password == user.Password).FirstOrDefault();
                 if (result != null)
                 {*/
                using (ArchnetsITHelpDesk dc = new ArchnetsITHelpDesk())
                {
                    var checkcrete = dc.UserRegisters.Where(x => x.Email == user.EmailID && x.Password == user.Password && x.registercount == 1).FirstOrDefault();
                    
                    if (checkcrete != null)
                    {
                        ViewBag.data = checkcrete.Email;
                        TempData["data"] = checkcrete.Email;
                        TempData["data1"] = checkcrete;
                        return RedirectToAction("CretePassword", "UserRegister");
                    }
                    else
                    {
                        var getarecord = dc.UserRegisters.Where(a => a.Email == user.EmailID && a.Password == user.Password).FirstOrDefault();
                        if (getarecord != null)
                        {
                            if (getarecord.RoleType == 2)
                            {
                               
                                    FormsAuthentication.SetAuthCookie(user.EmailID, false);
                                    return RedirectToAction("GetEmployeeRecords", "UserRegister");
                                
                              
                            }
                            else
                            {
                               
                                    FormsAuthentication.SetAuthCookie(user.EmailID, false);
                                    return RedirectToAction("Getallemployees", "Admin");
                                
                               
                            }
                        }
                        else
                        {
                            message = "Invalid Username And Password";
                        }
                       
                        
                    }
                
                /*  }
                      else
                      {
                          message = "Invalid credential provided";
                      }*/
                ViewBag.Message = message;
            }
            return View();
        }
        [HttpGet]
        public ActionResult CretePassword1()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CretePassword1(CreatePassword password)
        {
            try
            {

                var otpsentsuccessfully = TempData["OtpVerifySuccessfully is"];
                var email = TempData["Email"];
                var result = db.UserRegisters.FirstOrDefault(x => x.Email == email);
                if (result != null)
                {
                    result.Password = password.NewPassword;
                    result.registercount = null;
                    db.SaveChanges();
                    TempData["NewPassword"] = "  New Password Created Successfully";
                    return RedirectToAction("Login", "UserRegister");
                }

                return View();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);  
            }
            return View();  
        }
        // Crete New Password View
        [HttpGet]
        public ActionResult CretePassword()
        {
            return View();
        }
        // create password Functionality
        [HttpPost]
        /*[ValidateAntiForgeryToken]*/
        public ActionResult CretePassword(CreatePassword user)
        {

            try
            {
                var result22 = TempData["data"];
                var resul44 = TempData["data1"];
                var result = db.UserRegisters.Where(x => x.Email == result22).FirstOrDefault();
                if (result != null)
                {
                    result.Password = user.NewPassword;
                    result.registercount = null;
                    db.SaveChanges();
                    // Session["Successfull"] = "Password SuccessFully  Created Continue to Login...";
                    TempData["Successfull"] = "Password SuccessFully  Created Continue to Login...";
                    return RedirectToAction("Login", "UserRegister");
                }
                return View();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);  
            }
            return View();
            
           
        }
        // forgot password view 
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        // forgot password functionality
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPassword password)
        {
           
                bool Status = false;
                string message1 = "";
                //  var result = db.UserRegisters.ToList();
                //var resltu = db.UserRegisters.FirstOrDefault(x => x.Email == .Select(x => x.Email).FirstOrDefault());
                var result = db.UserRegisters.Where(x => x.Email == password.MobileNumberAndEmail || x.Phone_Number == password.MobileNumberAndEmail).FirstOrDefault();
                if (result != null)
                {
                    var number = GetNumber(password.MobileNumberAndEmail);
                    if (number)
                    {
                        var otp = ViewBag.otp;
                        const string accountSid = "AC5b65569ae8d450277b340edc081cd925";
                        const string authToken = "b624391854339cf4aa75bd55ac2dabd8";
                        TwilioClient.Init(accountSid, authToken);
                        var to = new PhoneNumber("+91" + password.MobileNumberAndEmail);
                        var message = MessageResource.Create(
                        to,

                        from: new PhoneNumber("+19546377093"),
                        body: $"Verification OTP " + otp + "");
                        password.otp = otp;
                        result.Otp = otp;
                        db.SaveChanges();
                        ViewBag.message = "Otp Sent to your mobile Number";
                        ViewBag.message = "Otp Sent to your mobile Number";
                        ViewBag.message = "Otp Sent to your mobile Number";
                        ViewBag.message = "Otp Sent to your mobile Number";
                    }
                    else
                    {
                        //  int EmailOtp = new Random().Next(100000, 999999);

                        var Email = SendVerificationLinkEmail1(password.MobileNumberAndEmail);
                        var result111 = TempData["Otp"];
                        password.otp = ViewBag.otp;
                        result.Otp = ViewBag.otp;
                        db.SaveChanges();
                        /* message1 = "Registration successfully done. password " +
                                 " has been sent to your email id:" + password.MobileNumberAndEmail;*/
                      /*  Session["successmessage"] = "Successfully sent otp your Email";*/
                        TempData["successmessage"]= "otp  has been successfully sent to your mail";
                        /* Status = true;
                         TempData["message"] = message1;*/

                        return RedirectToAction("Verifyotp", "UserRegister");
                    }
                }
                else
                {
                    ViewBag.message = "Please Enter Valid Email";
                }
           
            return View();
        }
        // sent verification otp in register emailId
        [NonAction]
        public bool SendVerificationLinkEmail1(string emailID)
        {
            int otpValue = new Random().Next(100000, 999999);
            ViewBag.otp=otpValue;
            TempData["Otp"]=otpValue;   
            // var verifyUrl = "/Account/" +"/" ;
            // var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var fromEmail = new MailAddress("Complaints.archents@outlook.com", emailID);
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "V@11@pu6eddy$"; // Replace with actual password
            string subject = "";
            string body = "";

            subject = "OTP";
            body = "<br/><br/>  <br/>  <b>Your Password is:</b>" + " " + "" + " "
               + otpValue + "" + "</a> ";


            MailMessage mc = new MailMessage("Complaints.archents@outlook.com", emailID);
            mc.Subject = subject;
            mc.Body = body;
            mc.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
            smtp.Timeout = 1000000;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            NetworkCredential nc = new NetworkCredential("Complaints.archents@outlook.com", "V@11@pu6eddy$");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = nc;
            smtp.Send(mc);
            return mc != null;
        }
        [NonAction]
        public bool GetNumber(string Number)
        {
            var getnumber = db.UserRegisters.Where(x => x.Phone_Number == Number).FirstOrDefault();
            return getnumber != null;
        }
        // Verify Otp
        [HttpGet]
        public ActionResult Verifyotp()
        {
            return View();  
        }
        [HttpPost]
        public ActionResult Verifyotp(VerifyOtp opt)
        {

            try
            {
                var result = db.UserRegisters.FirstOrDefault(x => x.Otp == opt.Otp);

                if (result != null)
                {
                    TempData["Email"] = result.Email;
                    result.Otp = null;
                    db.SaveChanges();
                    // Session["OtpVerifySuccessfully"] = "Otp Verify Successfully";
                    TempData["OtpVerifySuccessfully"] = "Otp Verify Successfully";
                    return RedirectToAction("CretePassword1", "UserRegister");
                }
                else
                {
                    ViewBag.message = "enter valid otp";
                }

                return View();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);  
                
            }
            return View();
        }
        [HttpGet]
        public ActionResult GetEmployeeRecords()
        {
            ViewBag.successfully = TempData["Successfully"];
         ViewBag.successaddticket=TempData["NewTicketCrated"];
        //  var result = db.UserRegisters.FirstOrDefault(x => x.Id == User.Identity.GetUserId());

            var result11 = db.UserRegisters.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().EmpID;
            var result = db.RaiseTickets.Where(x=>x.EmployeeId==result11).ToList();
            TempData["AuthenticateName"] = db.UserRegisters.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().FirstName;
            Session["LoginName"]= db.UserRegisters.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().FirstName;
            ViewBag.registerid = result11;

            if (result.Count == 0)
            {
                ViewBag.name = "No records found";
            }
                return View(result);
        }
        /*  [HttpGet]
          public ActionResult Getlist()
          {
              var result = db.RaiseTickets.ToList();
              return View(result);
          }*/

        [HttpGet]
        public ActionResult Profile()
        {
            var result11 = db.UserRegisters.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().EmpID;
            var Data=db.UserRegisters.FirstOrDefault(x=>x.EmpID==result11);


            return View(Data);
        }
        [HttpPost]
        public ActionResult Profile(UserRegister user)
        {
            if (user != null)
            {
                var result=db.UserRegisters.FirstOrDefault(x=>x.Email== user.Email);
                if (result.RoleType == 2)
                {
                    if (result != null)
                    {
                        result.FirstName = user.FirstName;
                        result.LastName = user.LastName;
                        result.Email = user.Email;
                        result.Designation = user.Designation;
                        result.Phone_Number = user.Phone_Number;
                        db.SaveChanges();
                       TempData["Successfully"] = "Your Profile Update Successfully";
                        return RedirectToAction("GetEmployeeRecords", "UserRegister");

                    }
                }
                else
                {
                    result.FirstName = user.FirstName;
                    result.LastName = user.LastName;
                    result.Email = user.Email;
                    result.Designation = user.Designation;
                    result.Phone_Number = user.Phone_Number;
                    db.SaveChanges();
                    TempData["Successfully"] = "Your Profile Update Successfully";
                    return RedirectToAction("Getallemployees", "Admin");
                }
                
            }
            return View();
        }

    }
   
}