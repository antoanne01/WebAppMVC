using AdminModule.Models;
using AdminModule.ViewModel;
using AutoMapper;
using BL.BLModels;
using BL.DALModels;
using BL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminModule.Controllers
{
    public class GenreController : Controller
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IGenreRepository _genreRepo;

        public GenreController(RwaMoviesContext dbContext, IMapper mapper, IGenreRepository genreRepo)
        {
            _dbContext = dbContext;
            _genreRepo = genreRepo;
            _mapper = mapper;
        }
        
        public ActionResult Index(int page, int size, string orderBy, string direction)
        {
            try
            {
                if(size == 0)
                {
                    size = 10;
                }

                var genre = _genreRepo.GetPagedData(page, size, orderBy, direction);
                var vmGenre = _mapper.Map<IEnumerable<VMGenre>>(genre);

                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["orderBy"] = orderBy;
                ViewData["direction"] = direction;
                ViewData["pages"] = _genreRepo.GetTotalCount() / size;

                return View(vmGenre);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public ActionResult GenreTableBodyPartial(int page, int size, string orderBy, string direction)
        {
            try
            {
                if (size == 0)
                {
                    size = 10;
                }

                var genre = _genreRepo.GetPagedData(page, size, orderBy, direction);
                var vmGenre = _mapper.Map<IEnumerable<VMGenre>>(genre);

                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["orderBy"] = orderBy;
                ViewData["direction"] = direction;
                ViewData["pages"] = _genreRepo.GetTotalCount() / size;

                return PartialView("_GenreTableBodyPartial", vmGenre);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: GenreController/Details/5
        public ActionResult Details(int id)
        {
            var genre = _genreRepo.GetGenreById(id);

            if (genre == null)
            {
                return NotFound();
            }

            var detailsViewModel = new VMGenre
            {
                Id = genre.Id,
                Name = genre.Name,
                Description = genre.Description
            };

            return View(detailsViewModel);
        }

        // GET: GenreController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GenreController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VMGenre genre)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Map the VMGenre to BLGenre
                    var blGenre = new BLGenre
                    {
                        Name = genre.Name,
                        Description = genre.Description
                    };

                    // Call the repository method to add the genre
                    var addedGenre = await _genreRepo.AddGenreAsync(blGenre);

                    return RedirectToAction("Index");
                }

                // If model validation fails, return the validation errors as JSON response
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return View(genre);
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an error JSON response
                return BadRequest(ex.Message);
            }
        }
    

        // GET: GenreController/Edit/5
        public ActionResult Edit(int id)
        {
            var genre = _genreRepo.GetGenreById(id);

            if (genre == null)
            {
                return NotFound();
            }

            // Map the video data to a view model if needed
            var editViewModel = _mapper.Map<VMGenre>(genre);

            return View(editViewModel);
        }

        // POST: GenreController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id,  VMEditGenre editGenre)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Retrieve the video from the repository or database based on the provided id
                    var genre = _genreRepo.GetGenreById(id);

                    if (genre == null)
                    {
                        return NotFound();
                    }

                    // Update the video properties with the values from the view model
                    genre.Name = editGenre.Name;
                    genre.Description = editGenre.Description;

                    // Save the changes to the repository or database
                    _genreRepo.UpdateGenre(genre);

                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: GenreController/Delete/5
        public ActionResult Delete(int id)
        {
            var genre = _genreRepo.GetGenreById(id);

            if (genre == null)
            {
                return NotFound();
            }

            var deleteViewModel = new VMGenre
            {
                Id = genre.Id,
                Name = genre.Name,
                Description = genre.Description
            };

            return View(deleteViewModel);
        }

        // POST: GenreController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, VMGenre deleteGenre)
        {
            try
            {
                var genre = _genreRepo.GetGenreById(id);

                if (genre == null)
                {
                    return NotFound();
                }

                _genreRepo.DeleteGenre(genre);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
