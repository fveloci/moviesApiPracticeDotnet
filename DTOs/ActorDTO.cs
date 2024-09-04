using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.DTOs
{
    public class ActorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateTime BirthDate { get; set; }
        public string Photo { get; set; }
    }
}
