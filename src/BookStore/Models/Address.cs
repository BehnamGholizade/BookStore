using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "CountryId is required")]
        public int CountryId { get; set; }
        [Required(ErrorMessage = "Country is required")]
        public Country Country { get; set; }

        [Required(ErrorMessage = "State is required")]
        public int StateId { get; set; }
        public State State { get; set; }

        [Required(ErrorMessage = "City is required")]
        public int CityId { get; set; }
        public City City { get; set; }

        [Required(ErrorMessage = "Zip is required")]
        public int ZipId { get; set; }
        public Zip Zip { get; set; }

        [Required(ErrorMessage = "Address Line is required")]
        public string AddressLine1 {get; set;}
        public string AddressLine2 { get; set; }

        [Required(ErrorMessage = "User is required")]
        [MaxLength(450)]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}