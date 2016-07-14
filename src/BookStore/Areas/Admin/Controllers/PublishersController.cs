using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Microsoft.AspNetCore.Mvc;
using BookStore.Data;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PublishersController : Controller
    {
        private UnitOfWork _uow { get; set; }
        private IMapper _mapper { get; set; }

        public PublishersController(UnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        // GET: Admin/Publishers
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            return Json(_uow.PublisherRepository.GetAll()
                .OrderBy(x => x.Name).ToDataSourceResult(request));
        }

        [HttpPost]
        public async Task<IActionResult> Create([DataSourceRequest]DataSourceRequest request, PublisherViewModel publisher)
        {
            if (ModelState.IsValid)
            {
                var publisherDb = _mapper.Map<Publisher>(publisher);

                _uow.PublisherRepository.Insert(publisherDb);
                await _uow.SaveChangesAsync();
                publisher.Id = publisherDb.Id;
            }

            return Json(new[] { publisher }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public async Task<IActionResult> Update([DataSourceRequest]DataSourceRequest request, PublisherViewModel publisher)
        {
            if (ModelState.IsValid)
            {
                var publisherDb = _mapper.Map<Publisher>(publisher);

                _uow.PublisherRepository.Update(publisherDb);
                await _uow.SaveChangesAsync();
            }
            return Json(new[] { publisher }.ToDataSourceResult(request, ModelState));
        }

        public async Task<IActionResult> Destroy([DataSourceRequest]DataSourceRequest request, PublisherViewModel publisher)
        {
            //publisher associated with the book(s)?
            bool associated = _uow.BookRepository.GetAll().Any(b => b.PublisherId == publisher.Id);
            if (associated)
            {
                ModelState.AddModelError(string.Empty, "You can not remove the publisher associated with book(s).");
            }

            if (ModelState.IsValid)
            {
                var publisherDb = _mapper.Map<Publisher>(publisher);

                _uow.PublisherRepository.Delete(publisherDb);
                await _uow.SaveChangesAsync();
            }
            return Json(new[] { publisher }.ToDataSourceResult(request, ModelState));
        }
    }
}
