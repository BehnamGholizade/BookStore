using BookStore.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BookStore.ViewModels
{
    public class AddressViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "CountryId is required")]
        public int CountryId { get; set; }
        public string CountryName { get; set; }

        [Required(ErrorMessage = "StateId is required")]
        public int StateId { get; set; }
        public string StateCode { get; set; }

        [Required(ErrorMessage = "CityId is required")]
        public int CityId { get; set; }
        public string CityName { get; set; }

        [Required(ErrorMessage = "ZipId is required")]
        public int ZipId { get; set; }
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Address Line 1 is required")]
        public string AddressLine1 {get; set;}
        public string AddressLine2 { get; set; }

        [Required]
        [MaxLength(450)]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}