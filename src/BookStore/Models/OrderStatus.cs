using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class OrderStatus
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Status name is required")]
        public string Name { get; set; }

        public string Description { get; set; }

        [JsonIgnore]
        public ICollection<Order> Orders { get; set; }
    }
}