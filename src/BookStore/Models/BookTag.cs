using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class BookTag
    {
        public int BookId { get; set; }
        [JsonIgnore]
        public Book Book { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}