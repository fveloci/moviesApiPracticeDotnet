using APIPeliculas.DTOs;
using APIPeliculas.Entities;
using APIPeliculas.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIPeliculas.Controllers
{
    [ApiController]
    [Route("api/movies")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFirebaseStorage firebaseStorage;
        private readonly UserManager<IdentityUser> userManager;

        public MoviesController(ApplicationDbContext context, IMapper mapper, IFirebaseStorage firebaseStorage, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.firebaseStorage = firebaseStorage;
            this.userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<LandingPageDTO>> Get()
        {
            var top = 6;
            var today = DateTime.Today;

            var comingSoon = await context.Movies
                .Where(x => x.ReleaseDate > today)
                .OrderBy(x => x.ReleaseDate)
                .Take(top)
                .ToListAsync();

            var onCinema = await context.Movies
                .Where(x => x.OnCinema == "true")
                .OrderBy(x => x.ReleaseDate)
                .Take(top)
                .ToListAsync();

            var result = new LandingPageDTO();
            result.ComingSoon = mapper.Map<List<MovieDTO>>(comingSoon);
            result.OnCinemas = mapper.Map<List<MovieDTO>>(onCinema);

            return result;
        }

        [HttpGet("PutGet/{id:int}")]
        public async Task<ActionResult<MoviesPutGetDTO>> PutGet(int id)
        {
            var movieActionResult = await Get(id);

            if (movieActionResult.Result is NotFoundResult)
            {
                return NotFound();
            }

            var movie = movieActionResult.Value;

            var selectedGenresIds = movie.Genres.Select(x => x.Id).ToList();
            var notSelectedGenres = await context.Genres
                .Where(x => !selectedGenresIds.Contains(x.Id))
                .ToListAsync();

            var selectedCinemasIds = movie.Cinemas.Select(x => x.Id).ToList();
            var notSelectedCinemas = await context.Cinemas
                .Where(x => !selectedCinemasIds.Contains(x.Id))
                .ToListAsync();

            var notSelectedGenresDTO = mapper.Map<List<GenreDTO>>(notSelectedGenres);
            var notSelectedCinemasDTO = mapper.Map<List<CinemaDTO>>(notSelectedCinemas);

            var response = new MoviesPutGetDTO();

            response.Movie = movie;
            response.SelectedGenres = movie.Genres;
            response.NotSelectedGenres = notSelectedGenresDTO;
            response.SelectedCinemas = movie.Cinemas;
            response.NotSelectedCinemas = notSelectedCinemasDTO;
            response.Actors = movie.Actors;

            return response;
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movie = await context.Movies
                .Include(x => x.MovieActors)
                .Include(x => x.MovieCinemas)
                .Include(x => x.MovieGenres)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            movie = mapper.Map(movieCreationDTO, movie);

            if (movieCreationDTO.Poster != null)
            {
                movie.Poster = await firebaseStorage.EditFile(movieCreationDTO.Poster, movie.Poster, Constants.movieImagesFolder);
            }

            insertActorOrder(movie);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<MovieDTO>> Get(int id)
        {
            var movie = await context.Movies
                .Include(x => x.MovieGenres).ThenInclude(x => x.Genre)
                .Include(x => x.MovieActors).ThenInclude(x => x.Actor)
                .Include(x => x.MovieCinemas).ThenInclude(x => x.Cinema)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (movie == null) return NotFound();

            var averageVote = 0.0;
            var userVote = 0;

            if (await context.Ratings.AnyAsync(x => x.MovieId == id))
            {
                averageVote = await context.Ratings.Where(x => x.MovieId == id)
                    .AverageAsync(x => x.Points);

                if(HttpContext.User.Identity.IsAuthenticated)
                {
                    var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
                    var user = await userManager.FindByEmailAsync(email);
                    var userId = user.Id;
                    var vote = await context.Ratings.FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == id);

                    if(vote != null)
                    {
                        userVote = vote.Points;
                    }
                }                
            }

            var dto = mapper.Map<MovieDTO>(movie);
            dto.UserVote = userVote;
            dto.AverageVote = averageVote;
            dto.Actors = dto.Actors.OrderBy(x => x.Order).ToList();
            return dto;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post([FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movie = mapper.Map<Movie>(movieCreationDTO);

            if (movieCreationDTO.Poster != null)
            {
                movie.Poster = await firebaseStorage.SaveFile(movieCreationDTO.Poster, Constants.movieImagesFolder);
            }

            insertActorOrder(movie);

            context.Add(movie);
            await context.SaveChangesAsync();
            return movie.Id;
        }
        [HttpGet("filter")]
        [AllowAnonymous]
        public async Task<ActionResult<List<MovieDTO>>> Filter([FromQuery] MoviesFilterDTO moviesFilterDTO)
        {
            var moviesQueryable = context.Movies.AsQueryable();

            if (!string.IsNullOrEmpty(moviesFilterDTO.Title))
            {
                moviesQueryable = moviesQueryable.Where(x => x.Title.Contains(moviesFilterDTO.Title));
            }

            if (moviesFilterDTO.OnCinema)
            {
                moviesQueryable = moviesQueryable.Where(x => x.OnCinema == "true");
            }

            if (moviesFilterDTO.ComingSoon)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(x => x.ReleaseDate > today);
            }

            if (moviesFilterDTO.GenreId != 0)
            {
                moviesQueryable = moviesQueryable
                    .Where(x => x.MovieGenres.Any(y => y.GenreId == moviesFilterDTO.GenreId));
            }

            await HttpContext.InsertHeaderPaginationParams(moviesQueryable);
            var movies = await moviesQueryable.Paginate(moviesFilterDTO.PaginationDTO).ToListAsync();
            return mapper.Map<List<MovieDTO>>(movies);
        }


        [HttpGet("PostGet")]
        public async Task<ActionResult<MoviePostGetDTO>> PostGet(int id)
        {
            var cinemas = await context.Cinemas.ToListAsync();
            var genres = await context.Genres.ToListAsync();

            var cinemasDTO = mapper.Map<List<CinemaDTO>>(cinemas);
            var genresDTO = mapper.Map<List<GenreDTO>>(genres);

            return new MoviePostGetDTO() { Cinemas = cinemasDTO, Genres = genresDTO };
        }

        private void insertActorOrder(Movie movie)
        {
            if (movie != null)
            {
                for (int i = 0; i < movie.MovieActors.Count; i++)
                {
                    movie.MovieActors[i].Order = i;
                }
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var movie = await context.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if(movie == null)
            {
                return NotFound();
            }

            context.Remove(movie);

            await context.SaveChangesAsync();

            await firebaseStorage.DeleteFile(movie.Poster, Constants.movieImagesFolder);
            return NoContent();
        }
    }
}
