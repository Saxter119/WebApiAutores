using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace webAPIAutores.Validaciones
{
    public class CapLetter: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if(value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var primeraLetraMayuscula = value.ToString()[0].ToString();

            if(primeraLetraMayuscula != primeraLetraMayuscula.ToUpper())
            {
                return new ValidationResult("La primera letra debe ser mayuscula");
            }

            return ValidationResult.Success;

        }
    }
}