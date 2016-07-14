using BookStore.Infrastructure;
using BookStore.Models;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BookStore.ViewComponents
{
    public class CartSummary : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
                var cart = HttpContext.Session.GetObjectFromJson<Cart>("Cart");
                if (cart == null)
                {
                    cart = new Cart();
                }

                var cartSummary = new CartSummaryViewModel
                {
                    Total = cart.CartLines.Sum(x => x.Quantity),
                    Sum = cart.ComputeTotalValue().ToString("c")
                };

            return View(cartSummary);
        }

    }
}
