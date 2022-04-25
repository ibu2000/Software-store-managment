using System.ComponentModel.DataAnnotations;

namespace SoftwareStoreManagment.Models
{
    public class Product 
    {
        [Key]
        public long ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Warranty { get; set; }

    }
}
