using AutoMapper;
using BookStore.Data;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStore.ViewModels;
using System.Threading;
using System.Collections.Generic;
using BookStore.Models;
using System.Net;

namespace BookStore.Controllers
{
    public class BooksController : Controller
    {
        private UnitOfWork _uow { get; set; }
        private IMapper _mapper { get; set; }

        public BooksController(UnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        // GET: /books
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //GET /books/books_read
        public async Task<IActionResult> Read([DataSourceRequest] DataSourceRequest request)
        {
            var books = await _uow.BookRepository.GetAll()
                .Include(ba => ba.BookAuthors).ThenInclude(a => a.Author)
                .Include(p => p.Publisher).ToListAsync();
            return Json(books.ToDataSourceResult(request));
        }

        // GET: /books/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _uow.BookRepository.GetAll()
                .Include(a => a.BookAuthors).ThenInclude(ba => ba.Author)
                .Include(p => p.Publisher)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: /books/Search?query=abc
        [HttpGet]
        public IActionResult Search(string query)
        {
            TempData["queryString"] = query;
            return View("Search", query);
        }

        public async Task<IActionResult> ReadSearchResult([DataSourceRequest] DataSourceRequest request)
        {
            string queryString = "";

            if (TempData.ContainsKey("queryString"))
                queryString = TempData["queryString"].ToString();

            if (string.IsNullOrWhiteSpace(queryString))
            {
                //return all books
                return await Read(request);
            }

            var books = await _uow.BookRepository.GetRequestedBooks(queryString)
                .Include(ba => ba.BookAuthors).ThenInclude(a => a.Author)
                .OrderByDescending(b => b.SearchRank)
                .Take(50)
                .ToListAsync();

            return Json(books.ToDataSourceResult(request));
        }

        // GET: /books/Bestsellers
        [HttpGet("Bestsellers")]
        public IActionResult Bestsellers()
        {
            return View();
        }

        public async Task<IActionResult> ReadBestsellers([DataSourceRequest] DataSourceRequest request)
        {
            var books = await _uow.BookRepository.GetBestsellers()
            .Include(ba => ba.BookAuthors).ThenInclude(a => a.Author).ToListAsync();
            return Json(books.ToDataSourceResult(request));
        }

        // GET: /books/ReadAlsoBought/bookId
        public async Task<IActionResult> ReadAlsoBought([DataSourceRequest] DataSourceRequest request, int bookId)
        {
            var books = await _uow.BookRepository.GetAlsoBought(bookId)
                .Distinct().Include(ba => ba.BookAuthors).ThenInclude(a => a.Author)
                .ToListAsync();
            return Json(books.ToDataSourceResult(request));
        }

        // GET: /books/BooksByAuthorId/5
        [HttpGet]
        public async Task<IActionResult> BooksByAuthorId(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _uow.AuthorRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            var authorVm = _mapper.Map<AuthorViewModel>(author);
            return View("BooksByAuthorId", authorVm);
        }

        public async Task<IActionResult> ReadBooksByAuthorId([DataSourceRequest] DataSourceRequest request, int authorId)
        {
            var books = await _uow.BookRepository.GetAll()
                .Include(a => a.BookAuthors).ThenInclude(ba => ba.Author)
                .Where(a => a.BookAuthors.Any(x => x.AuthorId == authorId))
                .ToListAsync();
            return Json(books.ToDataSourceResult(request));
        }

        // GET: /books/BooksByPublisherId/5
        [HttpGet]
        public async Task<IActionResult> BooksByPublisherId(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var publisher = await _uow.PublisherRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (publisher == null)
            {
                return NotFound();
            }

            var publisherVm = _mapper.Map<PublisherViewModel>(publisher);
            return View("BooksByPublisherId", publisherVm);
        }

        public async Task<IActionResult> ReadBooksByPublisherId([DataSourceRequest] DataSourceRequest request, int publisherId)
        {
            var books = await _uow.BookRepository.GetAll()
                .Include(a => a.BookAuthors).ThenInclude(ba => ba.Author)
                .Where(a => a.PublisherId == publisherId)
                .ToListAsync();
            return Json(books.ToDataSourceResult(request));
        }

        public async Task<IActionResult> ReadPublishers()
        {
            return Json(await _uow.PublisherRepository.GetAll().ToListAsync());
        }
    }
}
