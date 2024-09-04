using APIPeliculas.Entities;
using APIPeliculas.Utils;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.DTOs
{
    public class MovieCreationDTO
    {
        [Required]
        [StringLength(300)]
        public string Title { get; set; }
        public string Resume { get; set; }
        public string Trailer { get; set; }
        public string OnCinema { get; set; }
        public DateTime ReleaseDate { get; set; }
        public IFormFile Poster { get; set; }
        public List<MovieActor> MovieActors { get; set; }
        public List<MovieGenre> MovieGenres { get; set; }
        public List<MovieCinema> MovieCinemas { get; set; }
        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> GenreIds { get; set; }
        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> CinemasIds { get; set; }
        [ModelBinder(BinderType = typeof(TypeBinder<List<MovieActorCreationDTO>>))]
        public List<MovieActorCreationDTO> Actors { get; set; }
    }
}
