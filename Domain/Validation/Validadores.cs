#region IMPORTAÇÕES
using System;
using System.ComponentModel.DataAnnotations;
#endregion

namespace Domain.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class Validadores : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string fieldValue = value.ToString();

                if (validationContext.MemberName == "Placa")
                {
                    if (fieldValue.Length > 7)
                    {
                        return new ValidationResult("O campo Placa deve ter no máximo 7 caracteres.");
                    }
                }

                if (validationContext.MemberName == "Cor")
                {
                    foreach (char c in fieldValue)
                    {
                        if (!char.IsLetter(c))
                        {
                            return new ValidationResult("O campo Cor deve conter apenas letras.");
                        }
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
