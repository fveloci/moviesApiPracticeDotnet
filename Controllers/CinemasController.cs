using APIPeliculas.DTOs;
using APIPeliculas.Entities;
using APIPeliculas.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIPeliculas.Controllers
{
    [Route("api/cinemas")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public class CinemasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CinemasController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CinemaCreationDTO cinemaCreationDTO)
        {
            try
            {
                var cinema = mapper.Map<Cinema>(cinemaCreationDTO);
                context.Add(cinema);
                await context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<CinemaDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Cinemas.AsQueryable();
            await HttpContext.InsertHeaderPaginationParams(queryable);
            var cinemas = await queryable.OrderBy(x => x.Name).Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<CinemaDTO>>(cinemas);
        }

        [HttpGet("{Id:int}")]
        public async Task<ActionResult<CinemaDTO>> Get(int Id)
        {
            var cinema = await context.Cinemas.FirstOrDefaultAsync(x => x.Id == Id);
            if(cinema == null)
            {
                return NotFound();
            }
            return mapper.Map<CinemaDTO>(cinema);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int Id, [FromBody] CinemaCreationDTO cinemaCreationDTO)
        {
            var cinema = await context.Cinemas.FirstOrDefaultAsync(x => x.Id == Id);

            if (cinema == null)
            {
                return NotFound();
            }

            cinema = mapper.Map(cinemaCreationDTO, cinema);

            await context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await context.Cinemas.FirstOrDefaultAsync(x => x.Id == id);

            if (exist == null)
            {
                return NotFound();
            }

            context.Remove(exist);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
