using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.DTOs
{
    public class CinemaCreationDTO
    {
        [Required]
        [StringLength(75)]
        public string Name { get; set; }
        [Range(-90, 90)]
        public double Latitude { get; set; }
        [Range(-180, 180)]
        public double Longitude { get; set; }
    }
}
