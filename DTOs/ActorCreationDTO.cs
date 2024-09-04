using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.DTOs
{
    public class ActorCreationDTO
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateTime BirthDate { get; set; }
        public IFormFile Photo { get; set; }
    }
}
