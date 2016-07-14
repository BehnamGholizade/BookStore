using AutoMapper;
using BookStore.Data;
using BookStore.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BookStore.ViewModels;
using BookStore.Infrastructure;

namespace BookStore.Controllers
{
    //http://benjii.me/2015/07/using-sessions-and-httpcontext-in-aspnet5-and-mvc6/
    public class CartController : Controller
    {
        private UnitOfWork _uow { get; set; }
        private IMapper _mapper { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        private const string sessionKey = "Cart";

        public CartController(UnitOfWork uow, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _uow = uow;
            _mapper = mapper;
            _userManager = userManager;
        }

        public IActionResult GetCartViewComponent()
        {
            return ViewComponent("CartSummary");
        }

        [Route("Cart")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Checkout()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null) return View("LoginRequired");
            var user = _uow.UserRepository.GetById(x => x.Id == currentUser.Id);
            if (user != null)
            {
                var userVm = _mapper.Map<UserViewModel>(user);
                return View(userVm);
            }

            return View("LoginRequired");
        }

        public IActionResult ReadCartLines([DataSourceRequest] DataSourceRequest request)
        {
            var cart = GetCart().CartLines;
            return Json(cart.ToDataSourceResult(request));
        }

        [HttpPost]
        public IActionResult AddToCart(int bookId, int bookTypeId)
        {
            var product = _uow.BookRepository.GetAll().FirstOrDefault(x => x.Id == bookId);
            var bookType = _uow.BookTypeRepository.GetAll().FirstOrDefault(x => x.Id == bookTypeId);
            if (product != null && bookType != null)
            {
                var cart = GetCart();
                cart.AddItem(product, bookType, 1);
                HttpContext.Session.SetObjectAsJson(sessionKey, cart);
            }
            return GetCartViewComponent();
        }

        [HttpPost]
        public IActionResult RemoveFromCart([DataSourceRequest]DataSourceRequest request, CartLine cartLine)
        {
            var cart = GetCart();
            cart.RemoveLine(cartLine.Id);
            HttpContext.Session.SetObjectAsJson(sessionKey, cart);
            return Json(ModelState.ToDataSourceResult());
        }

        //TODO Use CartModelBinder
        private Cart GetCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<Cart>(sessionKey);
            if (cart == null)
            {
                cart = new Cart();
            }
            return cart;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(int? addressId)
        {
            var cart = GetCart();
            if (!cart.CartLines.Any()) return BadRequest("Your cart is empty.");

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (cart != null && currentUser.Id != null && cart.CartLines.Any() && addressId != null)
            {
                var order = new Order
                {
                    AddressId = addressId,
                    CreationDate = DateTime.Now,
                    Number = DateTime.Now.ToString("yyMMddHHmmss"),
                    UserId = currentUser.Id,
                    StatusId = 2,
                    Lines = new List<OrderLine>()
                };

                foreach (var cartLine in cart.CartLines)
                {
                    order.Lines.Add(new OrderLine
                    {
                        BookId = cartLine.Book.Id,
                        BookTypeId = cartLine.BookType.Id,
                        Price = cartLine.Price,
                        Quantity = cartLine.Quantity
                    });
                }

                order.TotalQty = order.Lines.Sum(x => x.Quantity);
                order.TotalSum = order.Lines.Sum(x => x.Quantity * x.Price);

                _uow.OrderRepository.Insert(order);
                await _uow.SaveChangesAsync();
                return Ok(order);
            }
            return BadRequest("An error occurred while creating the order.");
        }

    }
}
