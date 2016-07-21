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
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AuthorsController : Controller
    {
        private UnitOfWork _uow { get; set; }
        private IMapper _mapper { get; set; }

        public AuthorsController(UnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        // GET: Admin/Authors
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Read([DataSourceRequest]DataSourceRequest request)
        {
            var authors = await _uow.AuthorRepository.GetAll().ToListAsync();
            return Json(authors.ToDataSourceResult(request));
        }

        // Data for multiselect dropdownlist
        public async Task<IActionResult> GetBookAuthors()
        {
            // {"BookId": 0,
            //  "AuthorId": 5,<-
            //  "Author": {     |
            //    "Id": 5, -----
            //    "FirstName": xxx,
            //    "LastName": xxx,
            //    "About": xxx}
            Mapper.Initialize(cfg => cfg.CreateMap<Author, BookAuthor>()
                .ForMember(bt => bt.Author, opt => opt.MapFrom(src => src))
                .ForMember(bt => bt.AuthorId, opt => opt.MapFrom(src => src.Id)));

            return Json(await _uow.AuthorRepository.GetAll()
                .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                .ProjectTo<BookAuthor>().ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([DataSourceRequest]DataSourceRequest request,
            AuthorViewModel author)
        {
            if (ModelState.IsValid)
            {
                var authorDb = _mapper.Map<Author>(author);

                _uow.AuthorRepository.Insert(authorDb);
                await _uow.SaveChangesAsync();
                author.Id = authorDb.Id;
            }

            return Json(new[] { author }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public async Task<IActionResult> Update([DataSourceRequest]DataSourceRequest request,
            AuthorViewModel author)
        {
            if (ModelState.IsValid)
            {
                var authorDb = _mapper.Map<Author>(author);

                _uow.AuthorRepository.Update(authorDb);
                await _uow.SaveChangesAsync();
            }
            return Json(new[] { author }.ToDataSourceResult(request, ModelState));
        }

        public async Task<IActionResult> Destroy([DataSourceRequest]DataSourceRequest request,
            AuthorViewModel author)
        {
            //author associated with the book(s)?
            bool associatedAuthor = _uow.AuthorRepository._ctx.BookAuthors.Any(a => a.AuthorId == author.Id);
            if (associatedAuthor)
            {
                ModelState.AddModelError(string.Empty, "You can not remove the author associated with book(s).");
            }

            if (ModelState.IsValid)
            {
                var authorDb = _mapper.Map<Author>(author);
                _uow.AuthorRepository.Delete(authorDb);
                await _uow.SaveChangesAsync();
            }
            return Json(new[] { author }.ToDataSourceResult(request, ModelState));
        }
    }
}
