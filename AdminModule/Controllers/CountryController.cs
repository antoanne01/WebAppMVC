using AdminModule.ViewModel;
using AutoMapper;
using BL.BLModels;
using BL.DALModels;
using BL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminModule.Controllers
{
    public class CountryController : Controller
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ICountryRepository _countryRepo;

        public CountryController(RwaMoviesContext dbContext, IMapper mapper, ICountryRepository countryRepo)
        {
            _dbContext = dbContext;
            _countryRepo = countryRepo;
            _mapper = mapper;
        }

        public IActionResult Index(int page, int size, string orderBy, string direction)
        {
            try
            {
                if (size == 0)
                {
                    size = 10;
                }

                var blCountries = _countryRepo.GetPagedData(page, size, orderBy, direction);
                var vmCountries = _mapper.Map<IEnumerable<VMCountries>>(blCountries);

                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["orderBy"] = orderBy;
                ViewData["direction"] = direction;
                ViewData["pages"] = _countryRepo.GetTotalCount() / size;

                return View(vmCountries);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult CountryTableBodyPartial(int page, int size, string orderBy, string direction)
        {
            try
            {
                if (size == 0)
                {
                    size = 10;
                }

                var blCountries = _countryRepo.GetPagedData(page, size, orderBy, direction);
                var vmCountries = _mapper.Map<IEnumerable<VMCountries>>(blCountries);

                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["orderBy"] = orderBy;
                ViewData["direction"] = direction;
                ViewData["pages"] = _countryRepo.GetTotalCount() / size;

                return PartialView("_CountryTableBodyPartial", vmCountries);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public ActionResult Details(int id)
        {
            var country = _countryRepo.GetCountryById(id);

            if (country == null)
            {
                return NotFound();
            }

            var detailsViewModel = new VMCountries
            {
                Id = country.Id,
                Code = country.Code,
                Name = country.Name
            };

            return View(detailsViewModel);
        }

        // GET: CountryController/Create
        public ActionResult Create()
        {
            var countryOpt = _dbContext.Country.Select(g => g.Name).ToList();
            ViewBag.CountryOpt = countryOpt;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VMCountries country)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                var countryVar = _countryRepo.AddCountry(country.Code ,country.Name);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        public ActionResult Edit(int id)
        {
            var country = _countryRepo.GetCountryById(id);

            if (country == null)
            {
                return NotFound();
            }

            // Map the video data to a view model if needed
            var editViewModel = _mapper.Map<VMCountries>(country);

            return View(editViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, VMCountries editCountry)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Retrieve the video from the repository or database based on the provided id
                    var country = _countryRepo.GetCountryById(id);

                    if (country == null)
                    {
                        return NotFound();
                    }

                    // Update the video properties with the values from the view model
                    country.Code = editCountry.Code;
                    country.Name = editCountry.Name;

                    // Save the changes to the repository or database
                    _countryRepo.UpdateCountry(country);

                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: 
        public ActionResult Delete(int id)
        {
            var country = _countryRepo.GetCountryById(id);

            if (country == null)
            {
                return NotFound();
            }

            var deleteViewModel = new VMCountries
            {
                Id = country.Id,
                Code = country.Code,
                Name = country.Name
            };

            return View(deleteViewModel);
        }

        // POST: GenreController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, VMCountries deleteCountry)
        {
            try
            {
                var country = _countryRepo.GetCountryById(id);

                if (country == null)
                {
                    return NotFound();
                }

                _countryRepo.DeleteCountry(country);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
