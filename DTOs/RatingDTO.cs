using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.DTOs
{
    public class RatingDTO
    {
        public int MovieId { get; set; }
        [Range(1,5)]
        public int Points { get; set; }
    }
}
