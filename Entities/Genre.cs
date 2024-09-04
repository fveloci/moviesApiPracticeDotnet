using APIPeliculas.Validations;
using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.Entities
{
    public class Genre
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        [FirstCharacterUppercase]
        public string Name { get; set; }
        public List<MovieGenre> MovieGenre { get; set; }
    }
}
