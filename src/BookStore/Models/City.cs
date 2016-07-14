using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "City name is required")]
        [MaxLength(255)]
        public string Name { get; set; }

        public int? StateId { get; set; }
        [JsonIgnore]
        public State State { get; set; }

        //public Address Address { get; set; }

        [JsonIgnore]
        public ICollection<Zip> Zips { get; set; }
    }
}