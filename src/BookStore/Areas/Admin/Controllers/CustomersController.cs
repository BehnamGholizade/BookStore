using AutoMapper;
using BookStore.Data;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CustomersController : Controller
    {
        private UnitOfWork _uow { get; set; }
        private IMapper _mapper { get; set; }

        public CustomersController(UnitOfWork uow, IMapper mapper)
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

        public IActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            return Json(_uow.UserRepository.GetAll().ToDataSourceResult(request));
        }
    }
}
