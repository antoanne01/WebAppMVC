using AdminModule.Models;
using AdminModule.ViewModel;
using AutoMapper;
using BL.BLModels;
using BL.DALModels;
using BL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing;

namespace AdminModule.Controllers
{
    public class ConfirmedUserController : Controller
    {

        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepo;

        public ConfirmedUserController(RwaMoviesContext dbContext, IMapper mapper, IUserRepository userRepo)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userRepo = userRepo;
        }

        // GET: ConfirmedUserController
        public ActionResult Index(string firstNameFilter, string lastNameFilter, string usernameFilter, string countryFilter)
        {
            try
            {
                var users = _userRepo.GetAllConfirmed();


                if (!string.IsNullOrEmpty(firstNameFilter))
                {
                    users = users.Where(u => u.FirstName.Contains(firstNameFilter));
                }

                if (!string.IsNullOrEmpty(lastNameFilter))
                {
                    users = users.Where(u => u.LastName.Contains(lastNameFilter));
                }

                if (!string.IsNullOrEmpty(usernameFilter))
                {
                    users = users.Where(u => u.Username.Contains(usernameFilter));
                }

                if (!string.IsNullOrEmpty(countryFilter))
                {
                    int countryId;
                    if (int.TryParse(countryFilter, out countryId))
                    {
                        users = users.Where(u => u.CountryOfResidenceId == countryId);
                    }
                }

                var vmUser = _mapper.Map<IEnumerable<VMUser>>(users);

                var countries = _userRepo.GetCountries();
                ViewBag.Countries = countries.ToDictionary(c => c.Id, c => c.Name);

                return View(vmUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: ConfirmedUserController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var user = _userRepo.GetUserById(id);

                if (user == null)
                {
                    return NotFound();
                }

                var detailsViewModel = new VMUser
                {
                    Id = user.Id,
                    CreatedAt = user.CreatedAt,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.Phone,
                    CountryOfResidenceId = user.CountryOfResidenceId
                };

                var country = _userRepo.GetCountryById(user.CountryOfResidenceId);
                
                if (country != null)
                {
                    detailsViewModel.CountryName = country.Name;
                }

                return View(detailsViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: ConfirmedUserController/Create
        public ActionResult Create()
        {
            try
            {
                var countries = _userRepo.GetCountries();
                var createUser = new VMUser
                {
                    Countries = countries
                };

                return View(createUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: ConfirmedUserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VMUser user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(user);

                int countryId = user.CountryId;

                var userAdd = _userRepo.CreateUser(
                    user.Username,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Password,
                    user.Phone,
                    countryId);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: ConfirmedUserController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var countries = _userRepo.GetCountries();
                var editUser = new VMUser
                {
                    Countries = countries
                };

                return View(editUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: ConfirmedUserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, VMUser userConfirmed)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return NotFound();
                }

                var user = _userRepo.GetUserById(id);

                if (user == null)
                {
                    return NotFound();
                }

                user.Username = userConfirmed.Username;
                user.FirstName = userConfirmed.FirstName;
                user.LastName = userConfirmed.LastName;
                user.Email = userConfirmed.Email;
                user.Phone = userConfirmed.Phone;
                user.IsConfirmed = true;
                user.CountryOfResidenceId = userConfirmed.CountryId;

                _userRepo.UpdateConfirmedUser(user);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

            // GET: ConfirmedUserController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var user = _userRepo.GetUserById(id);

                if (user == null)
                {
                    return NotFound();
                }

                var deleteViewModel = new VMUser
                {
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                };

                return View(deleteViewModel);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        // POST: ConfirmedUserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, VMUser deleteUser)
        {
            try
            {
                var user = _userRepo.GetUserById(id);

                if (user == null)
                {
                    return NotFound();
                }

                _userRepo.DeleteUser(user);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
