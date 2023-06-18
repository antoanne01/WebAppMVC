using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RWA_IntegrationPart.Models;

namespace RWA_IntegrationPart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly RwaMoviesContext _dbRwaMovies;

        public GenreController(RwaMoviesContext dbRwaMovies)
        {
            _dbRwaMovies = dbRwaMovies;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Genre>> GetAllGenres()
        {
            try
            {
                return Ok(_dbRwaMovies.Genres);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "There has been a problem while fetching the data you requested");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Genre> GetGenre(int id)
        {
            try
            {
                var genre = _dbRwaMovies.Genres.FirstOrDefault(x => x.Id == id);
                if (genre == null)
                {
                    return NotFound("Requested genre not found");
                }
                return Ok(genre);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "There has been a problem while fetching the data you requested");
            }
            
        }

        [HttpPost]
        public ActionResult<Genre> AddGenre(Genre genre)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _dbRwaMovies.Genres.Add(genre);
                _dbRwaMovies.SaveChanges();

                return Ok(genre);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "There has been a problem while fetching the data you requested");
            }
            
        }

        [HttpPut("{id}")]
        public IActionResult UpdateGenre(int id, Genre genre)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = _dbRwaMovies.Genres.FirstOrDefault(x => x.Id == id);
                if (result == null)
                {
                    return NotFound("Could not find requested genre");
                }

                result.Name = genre.Name;
                result.Description = genre.Description;

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
        public IActionResult DeleteGenre(int id)
        {
            try
            {
                var genre = _dbRwaMovies.Genres.FirstOrDefault(x => x.Id == id);
                if (genre == null)
                {
                    return NotFound("Could not find requested genre");
                }

                _dbRwaMovies.Genres.Remove(genre);
                _dbRwaMovies.SaveChanges();

                return Ok(genre);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "There has been a problem while fetching the data you requested");
            }
            
        }
    }
}
