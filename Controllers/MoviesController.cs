using GenersOfMoviesAPIS.APPDbContext;
using GenersOfMoviesAPIS.Entities;
using GenersOfMoviesAPIS.EntitiesDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GenersOfMoviesAPIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {

        private readonly AppDbContext _context;
        private List<string> TypeOfFile = new List<string>() { "JPG" };
        private long lengthOfFile = 1048576 * 2;


        public MoviesController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> getAllMovies()
        {
            var query = await _context.movies.
                             Include(x => x.genere)
                             .Select(x => new { x.Title, GenereType = x.genere.Name, x.Id, x.Rate, x.StoreLine, x.year, x.genereId, x.poster }).
                             ToListAsync();

            if (query is null)
                return NotFound("not exist Movies");

            return Ok(query);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> getMovie(int id)
        {
            var search = await _context.movies.FindAsync(id);

            if (search is null)
                return NotFound("this Movie doesn't exist");

            return Ok(search);
        }



        [HttpPost]
        public async Task<IActionResult> addMovie([FromForm] MovieDTO dto)
        {
          

            using var dataStream = new MemoryStream();

            await dto.poster.CopyToAsync(dataStream);

            Movie movie = new Movie()
            {
                poster = dataStream.ToArray(),
                Rate = dto.Rate,
                StoreLine = dto.StoreLine,
                Title = dto.Title,
                year = dto.year,

            };

            if (dto.poster.Length > lengthOfFile)
                return BadRequest("lenght must be 1M");

            if (!TypeOfFile.Contains(Path.GetExtension(dto.poster.FileName)))
              return BadRequest("type of file not allowed must be JPG");

            if (movie is null)
                return BadRequest("the movie is null");

            if (!ModelState.IsValid)
                return BadRequest("Model state is ivalid");

            else
            {

                await _context.movies.AddAsync(movie);
                _context.SaveChanges();
            }

            return Created("", movie);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> updateMovie([FromForm] MovieDTO dto, int id)
        {

            using var memoryStream = new MemoryStream();
            await dto.poster.CopyToAsync(memoryStream);

            var query = await _context.generes.AnyAsync(x => x.Id == dto.genereId);
            var search = await _context.movies.FindAsync(id);

            if (query is false)
                return BadRequest("this genere id is not exist");

            if (dto.poster.Length > lengthOfFile)
                return BadRequest("lenght of file must be 1M");

            if (!TypeOfFile.Contains(Path.GetExtension(dto.poster.FileName).ToLower()))
                return BadRequest("type of file must be JPG");

            if (search is null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest("Model state is ivalid");

            else
            {

                search.poster = memoryStream.ToArray();
                search.Rate = dto.Rate;
                search.StoreLine = dto.StoreLine;
                search.Title = dto.Title;
                search.year = dto.year;

                _context.movies.Update(search);
                _context.SaveChanges();
            }

            return Ok(search);


        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletemovie(int id)
        {

            var search = await _context.movies.FindAsync(id);

            if (search is null)
                return BadRequest("the movie is null");

            _context.movies.Remove(search);
            _context.SaveChanges();

            return Ok(search);
        }


        [HttpGet("{GenereId}")]
        public async Task<IActionResult> getMovieAsync(int generId)
        {
            var search=await _context.movies.Where(x=>x.genereId==generId).Select(x=>new {x.Title,x.genereId}).ToListAsync();

            if (search is null)
                return NotFound("Not exist Movies with this genereId");

            return Ok(search);
        }

    }
}
