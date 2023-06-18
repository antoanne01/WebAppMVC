using AutoMapper;
using BL.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PublicModule.ViewModel;
using System.Security.Claims;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;

namespace PublicModule.Controllers
{
    public class UserController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepo;
        private readonly IMoviesRepository _movieRepo;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IImageRepository _imageRepo;

        public UserController(IUserRepository userRepo, IMapper mapper, IMoviesRepository movieRepo, IWebHostEnvironment hostingEnvironment, IImageRepository imageRepo)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _movieRepo = movieRepo;
            _hostingEnvironment = hostingEnvironment;
            _imageRepo = imageRepo;
        }

        public IActionResult Index()
        {
            var blUsers = _userRepo.GetAll();
            var vmUsers = _mapper.Map<IEnumerable<VMUser>>(blUsers);

            return View(vmUsers);
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("ChooseVideoContent", "User");
            }
            try
            {
                var countries = _userRepo.GetCountries();
                var register = new VMRegister
                {
                    Countries = countries
                };

                return View(register);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        public IActionResult Register(VMRegister register)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(register);

                int countryId = register.CountryId;

                var user = _userRepo.CreateUser(
                    register.Username,
                    register.FirstName,
                    register.LastName,
                    register.Email,
                    register.Password,
                    register.Phone,
                    countryId);

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(VMLogin login)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(login);

                var user = _userRepo.GetConfirmedUser(
                    login.Username,
                    login.Password);

                if (user == null)
                {
                    ModelState.AddModelError("Username", "Invalid username or password");
                    return View(login);
                }

                var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Username) };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties()).Wait();

                return RedirectToAction("ChooseVideoContent");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult ChooseVideoContent(string nameFilter, string descriptionFilter, int page, int size, string orderBy, string direction)
        {
            try
            {
                if (size == 0)
                {
                    size = 9;
                }

                var videos = _movieRepo.GetAll();

                // Filtering by name & description
                if (!string.IsNullOrEmpty(nameFilter))
                {
                    videos = videos.Where(v => v.Name.Contains(nameFilter));
                }

                if (!string.IsNullOrEmpty(descriptionFilter))
                {
                    videos = videos.Where(v => v.Description.Contains(descriptionFilter));
                }

                var totalVideos = videos.Count();

                // Paging
                var totalPages = (int)Math.Ceiling((double)totalVideos / size);
                page = Math.Max(0, Math.Min(page, totalPages - 1));

                videos = videos.Skip(page * size).Take(size);

                var videoViewModels = videos.Select(v => new ChooseVideoContent
                {
                    Id = v.Id,
                    Name = v.Name,
                    Description = v.Description,
                    ImageId = v.ImageId
                });

                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["orderBy"] = orderBy;
                ViewData["direction"] = direction;
                ViewData["pages"] = totalPages;

                return View(videoViewModels);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult VideoTableBodyPartial(string nameFilter, string descriptionFilter, int page, int size, string orderBy, string direction)
        {
            try
            {
                if (size == 0)
                {
                    size = 9;
                }

                var videos = _movieRepo.GetAll();

                // Filtering by name & description
                if (!string.IsNullOrEmpty(nameFilter))
                {
                    videos = videos.Where(v => v.Name.Contains(nameFilter));
                }

                if (!string.IsNullOrEmpty(descriptionFilter))
                {
                    videos = videos.Where(v => v.Description.Contains(descriptionFilter));
                }

                var totalVideos = videos.Count();

                // Paging
                var totalPages = (int)Math.Ceiling((double)totalVideos / size);
                page = Math.Max(0, Math.Min(page, totalPages - 1));

                videos = videos.Skip(page * size).Take(size);

                var videoViewModels = videos.Select(v => new ChooseVideoContent
                {
                    Id = v.Id,
                    Name = v.Name,
                    Description = v.Description,
                    ImageId = v.ImageId
                });

                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["orderBy"] = orderBy;
                ViewData["direction"] = direction;
                ViewData["pages"] = totalPages;

                return PartialView("_VideoTableBodyPartial", videoViewModels);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult VideoContent(int id)
        {
            try
            {
                var video = _movieRepo.GetVideoById(id);

                if (video == null)
                {
                    return NotFound();
                }

                var viewModel = new VMVideoContent
                {
                    Id = video.Id,
                    CreatedAt = video.CreatedAt,
                    Name = video.Name,
                    Description = video.Description,
                    GenreId = video.GenreId,
                    TotalSeconds = video.TotalSeconds,
                    StreamingUrl = video.StreamingUrl,
                    ImageId = video.ImageId
                };

                var genreMappings = _movieRepo.GetGenres();
                ViewBag.GenreMappings = genreMappings;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();

            return RedirectToAction("Login");
        }

        public IActionResult UserProfile()
        {
            var userProfile = _userRepo.GetUserByUsername(User.Identity.Name);

            var viewModel = new VMUserProfile
            {
                Id = userProfile.Id,
                CreatedAt = userProfile.CreatedAt,
                Username = userProfile.Username,
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                Email = userProfile.Email,
                Phone = userProfile.Phone,
                IsConfirmed = userProfile.IsConfirmed,
                CountryOfResidenceId = userProfile.CountryOfResidenceId
            };

            var countryMappings = _userRepo.GetCountry();
            ViewBag.CountryMappings = countryMappings;

            return View(viewModel);
        }

        public IActionResult ChangePassword(VMChangePassword changePassword)
        {
            if (!ModelState.IsValid)
            {
                return View(changePassword);
            }

            try
            {
                _userRepo.ChangePassword(changePassword.Username, changePassword.NewPassword);
                return RedirectToAction("ChooseVideoContent");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult Image(int id)
        {
            var blImage = _imageRepo.GetImage(id);
            byte[] imageData;

            if (blImage == null)
            {
                imageData = System.IO.File.ReadAllBytes("wwwroot/images/services-5.jpg");
            }
            else
            {
                imageData = Convert.FromBase64String(blImage.Content);
            }

            return new FileContentResult(imageData, "image/jpg");
        }
    }
}
