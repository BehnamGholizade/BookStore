using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Zip
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Zip is required")]
        [MaxLength(10)]
        public string Code { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal Longitude { get; set; }

        //public Address Address { get; set; }

        public int CityId { get; set; }
        [JsonIgnore]
        public City City { get; set; }
    }
}
