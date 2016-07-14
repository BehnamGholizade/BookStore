using BookStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.ViewModels
{
    public class OrderViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Order Number is required")]
        public string Number { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;

        [MaxLength(450)]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int? AddressId { get; set; }
        public Address Address { get; set; }

        public int StatusId { get; set; }
        public OrderStatus Status { get; set; }

        public int TotalQty { get; set; }

        public decimal TotalSum { get; set; }

        public ICollection<OrderLine> Lines { get; set; }
    }
}
