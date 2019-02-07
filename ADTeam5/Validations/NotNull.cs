using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.Validations
{
    public class NotNull: ValidationAttribute
    {
        private readonly string _other;

        public NotNull(string other)
        {
            _other = other;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_other);
            if (property == null)
            {
                return new ValidationResult(
                    string.Format("Please select a supplier")
                );
            }
           
            return ValidationResult.Success;
        }
    }
}
