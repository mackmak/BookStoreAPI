using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreAPI.Models
{
    public partial class Book
    {
        public int Id { get; set; }
        [Required]
        [StringLength(30)]
        public string Title{ get; set; }
        public int? Year { get; set; }
        [Required]
        [StringLength(20)]
        public string Isbn { get; set; }
        [Required]
        [StringLength(60)]
        public string Summary { get; set; }
        [Required]
        public string Image { get; set; }
        public double? Price { get; set; }

        public virtual Author Author { get; set; }
    }

}