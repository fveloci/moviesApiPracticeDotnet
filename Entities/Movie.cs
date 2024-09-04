using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        [Required]
        [StringLength(300)]
        public string Title { get; set; }
        public string Resume {  get; set; }
        public string Trailer { get; set; }
        public string OnCinema { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Poster {  get; set; }
        public List<MovieActor> MovieActors { get; set; }
        public List<MovieGenre> MovieGenres { get; set; }
        public List<MovieCinema> MovieCinemas { get; set; }
    }
}
