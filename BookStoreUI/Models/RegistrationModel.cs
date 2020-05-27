using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreUI.Models
{
    public class RegistrationModel : UserModel
    {

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation do not match")]
        public string ConfirmPassword { get; set; }

    }
}
