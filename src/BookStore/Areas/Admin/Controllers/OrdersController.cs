using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStore.Data;
using BookStore.Models;
using BookStore.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrdersController : Controller
    {
        private UnitOfWork _uow { get; set; }
        private IMapper _mapper { get; set; }

        public OrdersController(UnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        // GET: Orders
        [HttpGet]
        public IActionResult Index()
        {
            ViewData["orderStatuses"] = _uow.OrderStatusRepository.GetAll().OrderBy(s => s.Name);
            return View();
        }

        public IActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            return Json(_uow.OrderRepository.GetAll()
                .Include(x => x.User)
                .Include(x => x.Status)
                .ToDataSourceResult(request));
        }

        [AllowAnonymous]
        public IActionResult ReadChartData()
        {
            return Json(_uow.OrderRepository.GetAll()
                .Where(x => x.CreationDate.Year == DateTime.Now.Year)
                .GroupBy(x => new { x.CreationDate.Month, x.Status.Name })
                .Select(g => new OrderChartViewModel
                {
                    Order = g.Key.Month,
                    Month = new DateTime(1, g.Key.Month, 1).ToString("MMM", CultureInfo.InvariantCulture),
                    StatusName = g.Key.Name,
                    TotalSum = g.Sum(s => s.TotalSum)
                }).ToList());
        }

        public IActionResult ReadOrdersByUserId([DataSourceRequest]DataSourceRequest request, string userId)
        {
            return Json(_uow.OrderRepository.GetAll()
                .Include(x => x.User)
                .Include(x => x.Status)
                .Where(x => x.UserId == userId)
                .ToDataSourceResult(request));
        }

        public IActionResult ReadLines([DataSourceRequest]DataSourceRequest request, int orderId)
        {
            return Json(_uow.OrderLineRepository.GetAll()
                .Include(x => x.Book)
                .Include(x => x.BookType)
                .Where(x => x.OrderId == orderId)
                .ToDataSourceResult(request));
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([DataSourceRequest]DataSourceRequest request, Order order)
        {
            if (ModelState.IsValid)
            {
                _uow.OrderRepository.Insert(order);
                await _uow.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return Json(new[] { order }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Update([DataSourceRequest]DataSourceRequest request, Order order)
        {
            if (ModelState.IsValid)
            {
                _uow.OrderRepository.Update(order);
                await _uow.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return Json(new[] { order }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromBody]Order order)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (order.Id == 0)
                    _uow.OrderRepository.Insert(order);
                else
                    _uow.OrderRepository.Update(order);

                if (await _uow.SaveChangesAsync())
                {
                    return Ok(order);
                }

                return BadRequest("An error occurred while saving the order.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
