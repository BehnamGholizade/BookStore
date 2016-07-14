using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tag name is required")]
        [MaxLength(50)]
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<BookTag> BookTags { get; set; }
    }
}