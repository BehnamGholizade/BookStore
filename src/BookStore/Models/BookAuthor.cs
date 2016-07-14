using Newtonsoft.Json;

namespace BookStore.Models
{
    public class BookAuthor
    {
        public int BookId { get; set; }
        [JsonIgnore]
        public Book Book { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}
