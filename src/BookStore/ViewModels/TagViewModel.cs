using System.ComponentModel.DataAnnotations;

namespace BookStore.ViewModels
{
    public class TagViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tag name is required")]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
