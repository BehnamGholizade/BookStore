using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Cart
    {
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public ApplicationUser User { get; set; }
        public List<CartLine> CartLines { get; set; } = new List<CartLine>();

        public void AddItem(Book book, BookType bookType, int quantity)
        {
            var cartLine = CartLines.Where(b => (b.Book.Id == book.Id) && (b.BookType.Id == bookType.Id)).FirstOrDefault();

            if (cartLine == null)
            {
                CartLines.Add(new CartLine
                {
                    Id = (CartLines.Any()) ? CartLines.Max(x => x.Id) + 1 : 1,
                    Book = book,
                    BookType = bookType,
                    Quantity = quantity,
                    Price = (bookType.Id == 1) ? (book.PrintPrice ?? 0) : (book.EbookPrice ?? 0)
                });
            }
            else
            {
                cartLine.Quantity += quantity;
            }
        }

        public void RemoveLine(int lineId)
        {
            CartLines.RemoveAll(x => x.Id == lineId);
        }

        public decimal ComputeTotalValue()
        {
            return CartLines.Sum(b => b.Price * b.Quantity);
        }

        public void Clear()
        {
            CartLines.Clear();
        }
    }
}
