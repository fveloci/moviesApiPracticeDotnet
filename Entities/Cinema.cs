using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.Entities
{
    public class Cinema
    {
        public int Id { get; set; }
        [Required]
        [StringLength(75)]
        public string Name { get; set; }
        public Point Location { get; set; }
        public List<MovieCinema> MovieCinemas { get; set; }
    }
}
