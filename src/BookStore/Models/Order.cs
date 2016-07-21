using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Order Number is required")]
        public string Number { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreationDate { get; set; }

        [MaxLength(450)]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int? AddressId { get; set; }
        public Address Address { get; set; }

        public int StatusId { get; set; }
        public OrderStatus Status { get; set; }

        public int TotalQty { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalSum { get; set; }

        public ICollection<OrderLine> Lines { get; set; }

    }
}
