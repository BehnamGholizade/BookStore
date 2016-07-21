using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using AutoMapper;
using BookStore.ViewModels;
using BookStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Kendo.Mvc.UI;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BooksController : Controller
    {
        private UnitOfWork _uow { get; set; }
        private IMapper _mapper { get; set; }

        public BooksController(UnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        // GET: Admin/Books
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // GET: Admin/Books/Create
        public IActionResult Create()
        {
            var book = new BookViewModel();
            return View(book);
        }

        // POST: Admin/Books/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookViewModel bookVm)
        {
            if (ModelState.IsValid)
            {
                var book = _mapper.Map<Book>(bookVm);

                // Authors Multiselect
                for (int i = 0; i < bookVm.SelectedBookAuthorIds.Count(); i++)
                {
                    book.BookAuthors.Add(new BookAuthor { AuthorId = bookVm.SelectedBookAuthorIds[i] });
                }

                _uow.BookRepository.Insert(book);
                await _uow.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(bookVm);
        }

        // GET: Admin/Books/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _uow.BookRepository.GetAll()
                .Include(x => x.Publisher)
                .Include(x => x.BookAuthors).ThenInclude(x => x.Author)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            var bookVm = _mapper.Map<BookViewModel>(book);
            return View(bookVm);
        }

        // POST: Admin/Books/Edit/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookViewModel bookVm)
        {
            if (id != bookVm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var book = _mapper.Map<Book>(bookVm);

                // Authors Multiselect
                for (int i = 0; i < bookVm.SelectedBookAuthorIds.Count(); i++)
                {
                    book.BookAuthors.Add(new BookAuthor { AuthorId = bookVm.SelectedBookAuthorIds[i] });
                }

                _uow.BookRepository.Update(book);
                await _uow.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(bookVm);
        }

        public async Task<IActionResult> Destroy([DataSourceRequest]DataSourceRequest request, BookViewModel book)
        {
            //Is there the book in orders?
            bool condition = _uow.OrderLineRepository.GetAll().Any(x => x.BookId == book.Id);
            if (condition)
            {
                ModelState.AddModelError(string.Empty, "You can not remove the book associated with order(s).");
            }

            if (ModelState.IsValid)
            {
                var bookDb = _mapper.Map<Book>(book);

                _uow.BookRepository.Delete(bookDb);
                await _uow.SaveChangesAsync();
            }
            return Json(new[] { book }.ToDataSourceResult(request, ModelState));
        }
    }
}
