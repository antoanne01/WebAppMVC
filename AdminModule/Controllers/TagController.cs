using AdminModule.ViewModel;
using AutoMapper;
using BL.DALModels;
using BL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminModule.Controllers
{
    public class TagController : Controller
    {

        private readonly BL.DALModels.RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ITagRepository _tagRepo;

        public TagController(RwaMoviesContext dbContext, IMapper mapper, ITagRepository tagRepo)
        {
            _dbContext = dbContext;
            _tagRepo = tagRepo;
            _mapper = mapper;
        }

        // GET: TagController
        public ActionResult Index(int page, int size, string orderBy, string direction)
        {
            try
            {
                if (size == 0)
                    size = 10;

                var tags = _tagRepo.GetPagedData(page, size, orderBy, direction);
                var vmTag = _mapper.Map<IEnumerable<VMTag>>(tags);

                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["orderBy"] = orderBy;
                ViewData["direction"] = direction;
                ViewData["pages"] = _tagRepo.GetTotalCount() / size;

                return View(vmTag);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: TagController/Details/5
        public ActionResult Details(int id)
        {
            var video = _tagRepo.GetTagById(id);

            if (video == null)
            {
                return NotFound();
            }

            var detailsViewModel = new VMTag
            {
                Id = video.Id,
                Name = video.Name
            };

            return View(detailsViewModel);
        }

        // GET: TagController/Create
        public ActionResult Create()
        {
            var tagOptions = _dbContext.Tags.Select(g => g.Name).ToList();
            ViewBag.TagOptions = tagOptions;
            return View();
        }

        // POST: TagController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VMTag addTags)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                var tag = _tagRepo.BLAddTag(addTags.Name);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetNameById(int id)
        {
            var tag = _tagRepo.GetTagById(id);

            if (tag == null)
            {
                return NotFound();
            }

            return Ok(tag.Name);
        }

        // GET: TagController/Edit/5
        public ActionResult Edit(int id)
        {
            var tag = _tagRepo.GetTagById(id);

            if (tag == null)
            {
                return NotFound();
            }

            // Map the video data to a view model if needed
            var editViewModel = _mapper.Map<VMTag>(tag);

            return View(editViewModel);
        }

        // POST: TagController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, VMTag editTag)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Retrieve the video from the repository or database based on the provided id
                    var tag = _tagRepo.GetTagById(id);

                    if (tag == null)
                    {
                        return NotFound();
                    }

                    // Update the video properties with the values from the view model
                    tag.Name = editTag.Name;

                    // Save the changes to the repository or database
                    _tagRepo.UpdateTag(tag);

                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(editTag);
            }
        }

        // GET: TagController/Delete/5
        public ActionResult Delete(int id)
        {
            var tag = _tagRepo.GetTagById(id);

            if (tag == null)
            {
                return NotFound();
            }

            var deleteViewModel = new VMTag
            {
                Id = tag.Id,
                Name = tag.Name,
            };

            return View(deleteViewModel);
        }

        // POST: TagController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, VMTag deleteTag)
        {
            try
            {
                var tag = _tagRepo.GetTagById(id);

                if (tag == null)
                {
                    return NotFound();
                }

                _tagRepo.DeleteTag(tag);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
