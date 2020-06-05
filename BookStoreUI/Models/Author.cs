using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreUI.Models
{
    public class Author
    {

        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required]
        [StringLength(60)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(60)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(250)]
        [DisplayName("Biography")]
        public string Bio { get; set; }

        public virtual IList<Book> Books { get; set; }
    }
}
