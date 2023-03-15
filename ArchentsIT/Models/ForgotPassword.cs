using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace ArchentsIT.Models
{
    public class ForgotPassword
    {
        /*[Required(ErrorMessage = "Enter Email Or Mobile Number")]*/
        public string MobileNumberAndEmail { get; set; }
        public int? otp { get; set; }
    }
}