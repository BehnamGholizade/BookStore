using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class State
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "State name is required")]
        [MaxLength(255)]
        public string Name { get; set; }

        public string Code { get; set; }

        public int CountryId { get; set; }
        [JsonIgnore]
        public Country Country { get; set; }

        //public Address Address { get; set; }

        [JsonIgnore]
        public ICollection<City> Cities { get; set; }
    }
}