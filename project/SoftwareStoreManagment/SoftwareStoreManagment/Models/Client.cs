using System.ComponentModel.DataAnnotations;

namespace SoftwareStoreManagment.Models
{

    public class Client
    {
        [Key]
        public long ClientId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Phone { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }

    }
}
