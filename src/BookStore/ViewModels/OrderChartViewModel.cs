using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.ViewModels
{
    public class OrderChartViewModel
    {
        public int Order { get; set; }

        public string Month { get; set; }

        public string StatusName { get; set; }

        public decimal TotalSum { get; set; }
    }
}
