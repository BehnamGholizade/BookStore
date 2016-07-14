using AutoMapper;
using BookStore.Data;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStore.ViewModels;

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
        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_uow.BookRepository.GetAll()
                .Include(ba => ba.BookAuthors).ThenInclude(a => a.Author)
                .Include(p => p.Publisher)
                .ToDataSourceResult(request));
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
                .Include(t => t.BookTags).ThenInclude(bt => bt.Tag)
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
            return View("Search", query);
        }

        public IActionResult ReadSearchResult([DataSourceRequest] DataSourceRequest request, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                //return all books
                return Read(request);
            }

            return Json(_uow.BookRepository.GetRequestedBooks(query)
                .Include(ba => ba.BookAuthors).ThenInclude(a => a.Author)
                .ToDataSourceResult(request));
        }

        // GET: /books/Bestsellers
        [HttpGet("Bestsellers")]
        public IActionResult Bestsellers()
        {
            return View();
        }

        public IActionResult ReadBestsellers([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_uow.BookRepository.GetBestsellers()
            .Include(ba => ba.BookAuthors).ThenInclude(a => a.Author)
            .ToDataSourceResult(request));
        }

        // GET: /books/ReadAlsoBought/bookId
        public IActionResult ReadAlsoBought([DataSourceRequest] DataSourceRequest request, int bookId)
        {
            return Json(_uow.BookRepository.GetAlsoBought(bookId)
                .Distinct().Include(ba => ba.BookAuthors).ThenInclude(a => a.Author)
                .ToList()
                .ToDataSourceResult(request));
        }

        // GET: /books/BooksByAuthorId/5
        [HttpGet]
        public IActionResult BooksByAuthorId(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = _uow.AuthorRepository.GetAll()
                .FirstOrDefault(x => x.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            var authorVm = _mapper.Map<AuthorViewModel>(author);
            return View("BooksByAuthorId", authorVm);
        }

        public IActionResult ReadBooksByAuthorId([DataSourceRequest] DataSourceRequest request, int authorId)
        {
            return Json(_uow.BookRepository.GetAll()
                .Include(a => a.BookAuthors).ThenInclude(ba => ba.Author)
                .Where(a => a.BookAuthors.Any(x => x.AuthorId == authorId))
                .ToDataSourceResult(request));
        }

        // GET: /books/BooksByPublisherId/5
        [HttpGet]
        public IActionResult BooksByPublisherId(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var publisher = _uow.PublisherRepository.GetAll()
                .FirstOrDefault(x => x.Id == id);

            if (publisher == null)
            {
                return NotFound();
            }

            var publisherVm = _mapper.Map<PublisherViewModel>(publisher);
            return View("BooksByPublisherId", publisherVm);
        }

        public IActionResult ReadBooksByPublisherId([DataSourceRequest] DataSourceRequest request, int publisherId)
        {
            return Json(_uow.BookRepository.GetAll()
                .Include(a => a.BookAuthors).ThenInclude(ba => ba.Author)
                .Where(a => a.PublisherId == publisherId)
                .ToDataSourceResult(request));
        }

        public async Task<IActionResult> ReadPublishers()
        {
            return Json(await _uow.PublisherRepository.GetAll().ToListAsync());
        }
    }
}
