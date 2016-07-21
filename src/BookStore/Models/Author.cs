using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [NotMapped]
        public string FullName { get {
                return (FirstName + ' ' + LastName).Trim();
            }}

        public string About { get; set; }

        [JsonIgnore]
        public ICollection<BookAuthor> BookAuthors { get; set; }
    }
}