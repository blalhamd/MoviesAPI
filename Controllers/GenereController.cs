using GenersOfMoviesAPIS.APPDbContext;
using GenersOfMoviesAPIS.Entities;
using GenersOfMoviesAPIS.EntitiesDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GenersOfMoviesAPIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenereController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GenereController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> getAllGeners()
        {
            var query = await _context.generes.OrderBy(x => x.Name)
                .Include(x => x.Movies).Select(x => new {x.Name,x.Id,x.Movies})
                .ToListAsync();

            if(query is null)
                return NotFound("not exist generes");

            return Ok(query);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getGenere(int id)
        {
            var search = await _context.generes.FindAsync(id);

            if (search is null)
                return NotFound("this genere doesn't exist");
            
            return Ok(search);
        }

        [HttpPost]
        public async Task<IActionResult> addGenere([FromBody] GenereDTO dto) 
        {
            Genere genere = new Genere()
            {
                Name = dto.Name,
            };

            if (genere is null)
                return BadRequest("the genere is null");

            if (!ModelState.IsValid)
                return BadRequest("Model state is ivalid");

            else
            {
              
                await _context.generes.AddAsync(genere);
                _context.SaveChanges();
            }

            return Created("", genere);
        }

        [HttpPut]
        public async Task<IActionResult> updateGenere([FromBody] GenereDTO dto, int id)
        {

            var search = await _context.generes.FindAsync(id);

            if (search is null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest("Model state is ivalid");

            else
            {
               
                search.Name = dto.Name;

                _context.generes.Update(search);
                _context.SaveChanges();
            }

            return Ok(search);


        }


        [HttpDelete]
        public async Task<IActionResult> DeleteGenere(int id)
        {
          
            var search= await _context.generes.FindAsync(id);

            if (search is null)
                return BadRequest("the genere is null");

            _context.generes.Remove(search);

            return Ok(search);
        }



    }
}
