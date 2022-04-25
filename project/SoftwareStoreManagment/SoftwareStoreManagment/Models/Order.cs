using System.ComponentModel.DataAnnotations;

namespace SoftwareStoreManagment.Models
{
    public class Order 
    {

        [Key]
        public long OrderId { get; set; }
        [Required]
        public long ProductId { get; set; }
        public Product Product { get; set; }
        [Required]
        public long ClientId { get; set; }
        public Client Client { get; set; }
        [Required]
        public DateTime DateOfPurchase { get; set; } = DateTime.Now;

    }
}
