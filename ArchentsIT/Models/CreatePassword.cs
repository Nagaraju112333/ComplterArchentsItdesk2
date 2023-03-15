using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArchentsIT.Models
{
    public class CreatePassword
    {
       /* [Required(ErrorMessage = "New password required", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]*/
     //   [Required]
            public string NewPassword { get; set; }

/*        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New password and confirm password does not match")]

        [Required]*/

         public string ConfirmPassword { get; set; }
    }
}