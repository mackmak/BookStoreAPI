using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace BookStoreAPI.Models
{
    public partial class Author
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required]
        [StringLength(60)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(60)]
        public string LastName { get; set; }

        public string Bio { get; set; }

        public virtual IList<Book> Books { get; set; }
    }

}