using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RWA_IntegrationPart.Models;

namespace RWA_IntegrationPart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly RwaMoviesContext _dbRwaMovies;

        public TagsController(RwaMoviesContext dbRwaMovies)
        {
            _dbRwaMovies= dbRwaMovies;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Tag>> GetAllTags()
        {
            try
            {
                return _dbRwaMovies.Tags;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "There has been a problem while fetching the data you requested");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Tag> GetTag(int id)
        {
            try
            {
                var tag = _dbRwaMovies.Tags.FirstOrDefault(x => x.Id == id);
                if (tag == null)
                {
                    return NotFound("Requested tag could not be found");
                }
                return Ok(tag);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "There has been a problem while fetching the data you requested");
            }
            
        }

        [HttpPost]
        public ActionResult<Tag> AddTag(Tag tag)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _dbRwaMovies.Tags.Add(tag);
                _dbRwaMovies.SaveChanges();

                return Ok(tag);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "There has been a problem while fetching the data you requested");
            }
            
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTag(int id, Tag tag)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Requested tag cannot be found");
                }

                var result = _dbRwaMovies.Tags.FirstOrDefault(x => x.Id == id);
                if (result == null)
                {
                    return NotFound();
                }

                result.Name = tag.Name;
                _dbRwaMovies.SaveChanges();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "There has been a problem while fetching the data you requested");
            }
            
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTag(int id)
        {
            try
            {
                var tag = _dbRwaMovies.Tags.FirstOrDefault(x => x.Id == id);
                if (tag == null)
                {
                    return NotFound("Requested tag cannot be found");
                }

                _dbRwaMovies.Tags.Remove(tag);
                _dbRwaMovies.SaveChanges();

                return Ok(tag);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "There has been a problem while fetching the data you requested");
            }
            
        }
    }
}
