using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RWA_IntegrationPart.Models;

namespace RWA_IntegrationPart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly RwaMoviesContext _dbRwaMovies;

        public VideoController(RwaMoviesContext dbRwaMovies)
        {
            _dbRwaMovies = dbRwaMovies;
        }
 
        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Video>> GetALlVideos([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string name = null, [FromQuery] string orderBy = "id", [FromQuery] string orderDirection = "asc")
        {
            var result = _dbRwaMovies.Videos.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                result = result.Where(x => x.Name.Contains(name));
            }

            // Order by
            switch (orderBy.ToLower())
            {
                case "name":
                    result = orderDirection.ToLower() == "desc" ? result.OrderByDescending(v => v.Name) : result.OrderBy(v => v.Name);
                    break;
                case "totaltime":
                    result = orderDirection.ToLower() == "desc" ? result.OrderByDescending(v => v.TotalSeconds) : result.OrderBy(v => v.TotalSeconds);
                    break;
                default:
                    result = orderDirection.ToLower() == "desc" ? result.OrderByDescending(v => v.Id) : result.OrderBy(v => v.Id);
                    break;
            }

            // Get the page of data
            var result_page = result.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(result_page);
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Video>> SearchVideo(string videoName)
        {
            var result = _dbRwaMovies.Videos.Where(x => x.Name.Contains(videoName));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult<Video> GetOneVideo(int id)
        {
            var result = _dbRwaMovies.Videos.FirstOrDefault(x => x.Id == id);
            if(result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        // Adding video to database bellow
        
        [HttpPost]
        public ActionResult<Video> Post(Video video)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dbRwaMovies.Videos.Add(video);
            _dbRwaMovies.SaveChanges();

            return Ok(video);
        }
    }
}
