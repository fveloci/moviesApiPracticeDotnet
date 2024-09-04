using APIPeliculas.DTOs;
using APIPeliculas.Entities;
using APIPeliculas.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace APIPeliculas.Controllers
{
    [Route("api/actors")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public class ActorsController: ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFirebaseStorage firebaseStorage;
        private readonly ILogger<ActorsController> logger;

        public ActorsController(ApplicationDbContext context, IMapper mapper, IFirebaseStorage firebaseStorage, ILogger<ActorsController> logger) 
        {
            this._context = context;
            this._mapper = mapper;
            this.firebaseStorage = firebaseStorage;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreationDTO actorCreationDTO)
        {
            try
            {
                var actor = _mapper.Map<Actor>(actorCreationDTO);
                if (actorCreationDTO.Photo != null)
                {
                    actor.Photo = await firebaseStorage.SaveFile(actorCreationDTO.Photo, Constants.actorImagesFolder);
                }
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }

            
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            logger.LogInformation("Obteniendo lista de actores");
            var queryable = _context.Actors.AsQueryable();
            await HttpContext.InsertHeaderPaginationParams(queryable);
            var actors = await queryable.OrderBy(x => x.Name).Paginate(paginationDTO).ToListAsync();
            return _mapper.Map<List<ActorDTO>>(actors);
        }

        [HttpPost("searchByName")]
        public async Task<ActionResult<List<MovieActorDTO>>> SearchByName([FromBody] string name)
        {
            if (string.IsNullOrEmpty(name)) return new List<MovieActorDTO>();

            return await _context.Actors
                            .Where(x => x.Name.Contains(name))
                            .Select(x => new MovieActorDTO { Id = x.Id, Name = x.Name, Photo = x.Photo })
                            .Take(5)
                            .ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ActorDTO>> Get(int Id)
        {
            var actor = await _context.Actors.FirstOrDefaultAsync(x => x.Id == Id);
            if (actor == null) return NotFound();
            return _mapper.Map<ActorDTO>(actor);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int Id, [FromForm] ActorCreationDTO actorCreationDTO)
        {
            try
            {
                var actor = await _context.Actors.FirstOrDefaultAsync(x => x.Id == Id);
                if (actor == null) return NotFound();
                actor = _mapper.Map(actorCreationDTO, actor);

                if(actorCreationDTO.Photo != null)
                {
                    actor.Photo = await firebaseStorage.EditFile(actorCreationDTO.Photo, actor.Photo, Constants.actorImagesFolder);
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }            
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int Id)
        {
            try
            {
                var actor = await _context.Actors.FirstOrDefaultAsync(x => x.Id == Id);
                if (actor == null)
                {
                    return NotFound();
                }
                _context.Remove(actor);
                await firebaseStorage.DeleteFile(actor.Photo, Constants.actorImagesFolder);
                await _context.SaveChangesAsync();
                
                return NoContent();
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
            
        }
    }
}
