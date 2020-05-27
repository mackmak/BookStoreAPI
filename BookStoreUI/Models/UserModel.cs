using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreUI.Models
{
    public class UserModel
    {

        [Display(Name = "Email Address")]
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(maximumLength: 15, ErrorMessage = "Your password is limited to {1} characters", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[!@#$&*]).+$",
         ErrorMessage = "Password must have lowercase, uppercase and numeric characters")]
        public string Password { get; set; }
    }
}
