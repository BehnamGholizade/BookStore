using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using BookStore.ViewModels;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using BookStore.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TagsController : Controller
    {
        private UnitOfWork _uow { get; set; }
        private IMapper _mapper { get; set; }

        public TagsController(UnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        // GET: Admin/Tags
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            return Json(_uow.TagRepository.GetAll()
                .ToDataSourceResult(request));
        }

        // Data for multiselect dropdownlist
        public async Task<IActionResult> GetBookTags()
        {
            // {"BookId": 0,
            //  "TagId": 5,<-------
            //  "Tag": { "Id": 5, /
            //    "Name": xxx }
            Mapper.Initialize(cfg => cfg.CreateMap<Tag, BookTag>()
                .ForMember(bt => bt.Tag, opt => opt.MapFrom(src => src))
                .ForMember(bt => bt.TagId, opt => opt.MapFrom(src => src.Id)));

            return Json(await _uow.TagRepository.GetAll()
                .OrderBy(x => x.Name).ProjectTo<BookTag>().ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([DataSourceRequest]DataSourceRequest request, TagViewModel tag)
        {
            // replace non alphanumeric chars by space and remove extra spaces
            tag.Name = Regex.Replace(Regex.Replace(tag.Name, @"[^a-zA-Z0-9 ]", " ").Trim().ToUpperInvariant(), @"\s+", " ");

            //check tag.Name uniqueness
            bool tagExist = _uow.TagRepository.GetAll().Any(t => t.Name == tag.Name);
            if (tagExist)
            {
                ModelState.AddModelError(string.Empty, $"Tag \"{tag.Name}\" already exist.");
            }

            if (ModelState.IsValid)
            {
                var tagDb = _mapper.Map<Tag>(tag);

                _uow.TagRepository.Insert(tagDb);
                await _uow.SaveChangesAsync();
                tag.Id = tagDb.Id;
            }

            return Json(new[] { tag }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public async Task<IActionResult> Update([DataSourceRequest]DataSourceRequest request, TagViewModel tag)
        {
            // replace non alphanumeric chars by space and remove extra spaces
            tag.Name = Regex.Replace(Regex.Replace(tag.Name, @"[^a-zA-Z0-9 ]", " ").Trim().ToUpperInvariant(), @"\s+", " ");

            //check tag.Name uniqueness
            bool tagExist = _uow.TagRepository.GetAll().Any(t => t.Name == tag.Name);
            if (tagExist)
            {
                ModelState.AddModelError(string.Empty, $"Tag \"{tag.Name}\" already exist.");
            }

            if (ModelState.IsValid)
            {
                var tagDb = _mapper.Map<Tag>(tag);

                _uow.TagRepository.Update(tagDb);
                await _uow.SaveChangesAsync();
            }
            return Json(new[] { tag }.ToDataSourceResult(request, ModelState));
        }

        public async Task<IActionResult> Destroy([DataSourceRequest]DataSourceRequest request, TagViewModel tag)
        {
            //tag associated with the book(s)?
            bool associatedTag = _uow.TagRepository._ctx.BookTags.Any(t => t.TagId == tag.Id);
            if (associatedTag)
            {
                ModelState.AddModelError(string.Empty, "You can not remove the tag associated with book(s).");
            }

            if (ModelState.IsValid)
            {
                var tagDb = _mapper.Map<Tag>(tag);

                _uow.TagRepository.Delete(tagDb);
                await _uow.SaveChangesAsync();
            }
            return Json(new[] { tag }.ToDataSourceResult(request, ModelState));
        }
    }
}
