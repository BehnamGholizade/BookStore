using BookStore.Models;
using BookStore.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.ViewComponents
{
    public class CartDetail : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.GetObjectFromJson<Cart>("Cart");
            if (cart == null)
            {
                cart = new Cart();
            }

            return View(cart);
        }
    }
}
