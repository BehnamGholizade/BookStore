using BookStore.Models;

namespace BookStore.ViewModels
{
    public class OrderLineViewModel
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public int BookTypeId { get; set; }
        public BookType BookType { get; set; }
    }
}
