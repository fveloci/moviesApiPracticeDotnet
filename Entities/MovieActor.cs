using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.Entities
{
    public class MovieActor
    {
        public int MovieId { get; set; }
        public int ActorId { get; set; }
        public Movie Movie { get; set; }
        public Actor Actor { get; set; }
        [StringLength(100)]
        public string Character {  get; set; }
        public int Order { get; set; }
    }
}
