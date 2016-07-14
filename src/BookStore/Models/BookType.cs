using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class BookType
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Type name is required")]
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<OrderLine> OrderLines { get; set; }
    }
}