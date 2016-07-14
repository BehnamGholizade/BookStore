using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class CartLine
    {
        public int Id { get; set; }
        public Book Book { get; set; }
        public BookType BookType { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal
        {
            get
            {
                return Quantity * Price;
            }
            set { }
        }
    }
}
