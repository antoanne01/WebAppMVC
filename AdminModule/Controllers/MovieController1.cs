using AdminModule.Models;
using AdminModule.ViewModel;
using AutoMapper;
using BL.DALModels;
using BL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace AdminModule.Controllers
{
    public class MovieController1 : Controller
    {
        private readonly BL.DALModels.RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IMoviesRepository _movieRepo;
        private readonly IImageRepository _imageRepo;


        public MovieController1(RwaMoviesContext dbContext, IMapper mapper, IMoviesRepository movieRepo, IImageRepository imageRepo)
        {
            _dbContext = dbContext;
            _movieRepo = movieRepo;
            _mapper = mapper;
            _imageRepo = imageRepo;
        }

        // GET: MovieController1
        public ActionResult Index(string nameFilter, int page, int size, string orderBy, string direction)
        {
            try
            {
                if (size == 0)
                {
                    size = 10;
                }

                var videos = _movieRepo.GetAll();

                // Filtering by name & genre
                if (!string.IsNullOrEmpty(nameFilter))
                {
                    videos = videos.Where(v => v.Name.Contains(nameFilter));
                }

                var totalVideos = videos.Count();

                // Paging
                var totalPages = (int)Math.Ceiling((double)totalVideos / size);
                page = Math.Max(0, Math.Min(page, totalPages - 1));

                videos = videos.Skip(page * size).Take(size);

                var videoViewModels = videos.Select(v => new VMVideo
                {
                    Id = v.Id,
                    CreatedAt = v.CreatedAt,
                    Name = v.Name,
                    Description = v.Description,
                    GenreId = v.GenreId,
                    TotalSeconds = v.TotalSeconds,
                    StreamingUrl = v.StreamingUrl
                });

                var genreMappings = _movieRepo.GetGenres();

                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["orderBy"] = orderBy;
                ViewData["direction"] = direction;
                ViewData["pages"] = totalPages;

                ViewBag.Genres = genreMappings;

                return View(videoViewModels);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: MovieController1
        public IActionResult VideoTableBodyPartial(string nameFilter, int page, int size, string orderBy, string direction)
        {
            try
            {
                if (size == 0)
                {
                    size = 10;
                }

                var videos = _movieRepo.GetAll();

                // Filtering by name 
                if (!string.IsNullOrEmpty(nameFilter))
                {
                    videos = videos.Where(v => v.Name.Contains(nameFilter));
                }

                var blVideo = _movieRepo.GetPagedData(page, size, orderBy, direction);
                var vmVideo = _mapper.Map<IEnumerable<VMVideo>>(blVideo);

                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["orderBy"] = orderBy;
                ViewData["direction"] = direction;
                ViewData["pages"] = _movieRepo.GetTotalCount() / size;

                var genres = _movieRepo.GetGenres();
                ViewBag.Genres = genres;

                return PartialView("_VideoTableBodyPartial", vmVideo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // bellow method is to add video, once initial page is displayed, when clicked to add then go AddVIdeo page

        public IActionResult AddVideo()
        {
            try
            {
                var genreOptions = _dbContext.Genres.Select(g => g.Name).ToList();
                ViewBag.GenreOptions = genreOptions;
                return View();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // GET: MovieController1/Details/5
        public ActionResult Details(int id)
        {
            var video = _movieRepo.GetVideoById(id);

            if (video == null)
            {
                return NotFound();
            }

            var detailsViewModel = new VMDetails
            {
                Id = video.Id,
                Name = video.Name,
                Description = video.Description,
                GenreId = video.GenreId,
                TotalSeconds = video.TotalSeconds,
                StreamingUrl = video.StreamingUrl,
                GenreName = _movieRepo.GetGenres()[video.GenreId]
            };

            return View(detailsViewModel);
        }

        // GET: MovieController1/Create
        public ActionResult Create()
        {
            try
            {
                var genreOptions = _dbContext.Genres.Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name }).ToList();


                ViewBag.GenreOptions = genreOptions;
                return View();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: MovieController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VMAddVideo addVideo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                addVideo.ImageId = _imageRepo.CreateImage(addVideo.ImageUpload);

                var video = _movieRepo.BLAddVideo(
                    addVideo.Name,
                    addVideo.Description,
                    addVideo.GenreId,
                    addVideo.TotalSeconds,
                    addVideo.StreamingUrl,
                    addVideo.ImageId);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // GET: MovieController1/Edit/5
        public ActionResult Edit(int id)
        {
            var video = _movieRepo.GetVideoById(id);

            if (video == null)
            {
                return NotFound();
            }

            var genreOptions = _dbContext.Genres.Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name }).ToList();


            ViewBag.GenreOptions = genreOptions;
            return View();

        }

        // POST: MovieController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, VMEditVideo viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return NotFound();
                }

                // Retrieve the video from the repository or database based on the provided id
                var video = _movieRepo.GetVideoById(id);

                if (video == null)
                {
                    return NotFound();
                }

                // Update the video properties with the values from the view model
                video.Name = viewModel.Name;
                video.Description = viewModel.Description;
                video.GenreId = viewModel.GenreId;
                video.TotalSeconds = viewModel.TotalSeconds;
                video.StreamingUrl = viewModel.StreamingUrl;

                // Save the changes to the repository or database
                _movieRepo.UpdateVideo(video);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(viewModel);
            }
        }

        // GET: MovieController1/Delete/5
        public ActionResult Delete(int id)
        {
            var video = _movieRepo.GetVideoById(id);

            if (video == null)
            {
                return NotFound();
            }

            var deleteViewModel = new VMDeleteVideo
            {
                Id = video.Id,
                Name = video.Name,
            };

            return View(deleteViewModel);
        }

        // POST: MovieController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, VMDeleteVideo delete)
        {
            try
            {
                var video = _movieRepo.GetVideoById(id);

                if (video == null)
                {
                    return NotFound();
                }

                _movieRepo.DeleteVideo(video);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
