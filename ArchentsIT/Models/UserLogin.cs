using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace ArchentsIT.Models
{
    public class UserLogin
    {
       // [Display(Name = "EmailID")]
       /* [Required(ErrorMessage = "Please Enter EmailId")]*/
        public string EmailID { get; set; }
       /* [Required( ErrorMessage = "Please Enter Password")]*/
       // [DataType(DataType.Password)]
        public string Password { get; set; }
     //   public string Name { get; set; }
    }
}