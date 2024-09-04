using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.Validations
{
    public class FirstCharacterUppercase: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString())) {
                return ValidationResult.Success;
            }

            var firstCharacter = value.ToString()[0].ToString();

            if(firstCharacter != firstCharacter.ToUpper())
            {
                return new ValidationResult("La primera letra debe ser mayuscula");
            }
            return ValidationResult.Success;
        }
    }
}
