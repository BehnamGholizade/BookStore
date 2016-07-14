using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Country name is required")]
        [MaxLength(255)]
        public string Name { get; set; }

        //public Address Address { get; set; }

        [JsonIgnore]
        public ICollection<State> States { get; set; }
    }
}