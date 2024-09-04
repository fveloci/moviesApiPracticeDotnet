using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.DTOs
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Resume { get; set; }
        public string Trailer { get; set; }
        public bool OnCinema { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Poster { get; set; }
        public List<GenreDTO> Genres { get; set; }
        public List<MovieActorDTO> Actors { get; set; }
        public List<CinemaDTO> Cinemas { get; set; }
        public int UserVote {  get; set; }
        public double AverageVote { get; set; }
    }
}
