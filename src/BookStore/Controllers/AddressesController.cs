using AutoMapper;
using AutoMapper.QueryableExtensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookStore.Data;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;
using BookStore.ViewModels;

namespace BookStore.Controllers
{
    public class AddressesController : Controller
    {
        private UnitOfWork _uow { get; set; }
        private IMapper _mapper { get; set; }

        public AddressesController(UnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("Addresses")]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ReadCountries()
        {
            return Json(await _uow.CountryRepository.GetAll().ToListAsync());
        }

        public async Task<IActionResult> ReadStatesByCountryId(int? countryId)
        {
            return Json(await _uow.StateRepository.GetAll().Where(x => x.CountryId == countryId).ToListAsync());
        }

        public async Task<IActionResult> ReadCitiesByStateId(int? stateId)
        {
            return Json(await _uow.CityRepository.GetAll().Where(x => x.StateId == stateId).ToListAsync());
        }

        public async Task<IActionResult> ReadZipsByCityId(int? cityId)
        {
            return Json(await _uow.ZipRepository.GetAll().Where(x => x.CityId == cityId).ToListAsync());
        }

        public async Task<IActionResult> ReadAddressesByZipId(int? zipId)
        {
            return Json(await _uow.AddressRepository.GetAll().Where(x => x.ZipId == zipId).ToListAsync());
        }

        public IActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            return Json(_uow.AddressRepository.GetAll().ToList().ToDataSourceResult(request));
        }

        public IActionResult ReadAddressesByUserId([DataSourceRequest]DataSourceRequest request, string userId)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<Address, AddressViewModel>());

            return Json(_uow.AddressRepository.GetAll()
                .Include(x => x.Country).Include(x => x.State)
                .Include(x => x.City).Include(x => x.Zip)
                .Where(x => x.UserId == userId)
                .ProjectTo<AddressViewModel>()
                .ToList().ToDataSourceResult(request));
        }

        [HttpPost]
        public async Task<IActionResult> Create([DataSourceRequest] DataSourceRequest request, AddressViewModel address)
        {
            if (address != null && ModelState.IsValid)
            {
                var addressDb = _mapper.Map<Address>(address);
                _uow.AddressRepository.Insert(addressDb);
                await _uow.SaveChangesAsync();
            }

            return Json(new[] { address }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public async Task<IActionResult> Update([DataSourceRequest] DataSourceRequest request, AddressViewModel address)
        {
            if (address != null && ModelState.IsValid)
            {
                var addressDb = _mapper.Map<Address>(address);
                _uow.AddressRepository.Update(addressDb);
                await _uow.SaveChangesAsync();
            }

            return Json(new[] { address }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public async Task<IActionResult> Destroy([DataSourceRequest] DataSourceRequest request, AddressViewModel address)
        {
            //address associated with the order(s)?
            bool associatedAddress = _uow.OrderRepository.GetAll().Any(x => x.AddressId == address.Id);
            if (associatedAddress)
            {
                ModelState.AddModelError(string.Empty, "You can not remove the address associated with order(s).");
            }

            if (ModelState.IsValid && address != null)
            {
                var addressDb = _mapper.Map<Address>(address);
                _uow.AddressRepository.Delete(addressDb);
                await _uow.SaveChangesAsync();
            }

            return Json(new[] { address }.ToDataSourceResult(request, ModelState));
        }
    }
}

