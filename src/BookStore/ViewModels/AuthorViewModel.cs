using System.ComponentModel.DataAnnotations;

namespace BookStore.ViewModels
{
    public class AuthorViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        public string FullName
        {
            get { return FirstName + ' ' + LastName; }
            set { }
        }

        public string About { get; set; }
    }
}