using APIPeliculas.Validations;
using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.DTOs
{
    public class GenreCreationDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        [FirstCharacterUppercase]
        public string Name { get; set; }
    }
}
