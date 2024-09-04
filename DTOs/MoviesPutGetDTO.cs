namespace APIPeliculas.DTOs
{
    public class MoviesPutGetDTO
    {
        public MovieDTO Movie { get; set; }
        public List<GenreDTO> SelectedGenres { get; set; }
        public List<GenreDTO> NotSelectedGenres { get; set; }
        public List<CinemaDTO> SelectedCinemas { get; set; }
        public List<CinemaDTO> NotSelectedCinemas { get; set; }
        public List<MovieActorDTO> Actors { get; set; }

    }
}
