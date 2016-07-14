using System.ComponentModel.DataAnnotations;

namespace BookStore.ViewModels
{
    public class PublisherViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Publisher name is required")]
        public string Name { get; set; }
    }
}
