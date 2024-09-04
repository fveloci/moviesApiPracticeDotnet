using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.Entities
{
    public class Actor
    {
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateTime BirthDate { get; set; }
        public string Photo {  get; set; }
        public List<MovieActor> MovieActors { get; set; }
    }
}
