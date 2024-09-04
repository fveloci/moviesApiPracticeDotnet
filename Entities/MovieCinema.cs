namespace APIPeliculas.Entities
{
    public class MovieCinema
    {
        public int MovieId { get; set; }
        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; }
        public Movie Movie { get; set; }
    }
}
