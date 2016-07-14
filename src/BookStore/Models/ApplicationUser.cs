using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class ApplicationUser: IdentityUser
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [JsonIgnore]
        public ICollection<Address> Addresses { get; set; }

        [JsonIgnore]
        public ICollection<Order> Orders { get; set; }

    }
}
