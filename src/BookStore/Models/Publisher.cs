using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class Publisher
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Publisher name is required")]
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<Book> Books { get; set; }

    }
}