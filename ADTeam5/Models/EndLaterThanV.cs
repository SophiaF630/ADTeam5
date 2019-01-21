using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;

namespace ADTeam5.Models
{
    public class EndLaterThanV : ValidationAttribute
    {
        private string startdate;
        public EndLaterThanV(string StartDate)
        {
            startdate = StartDate;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

                var propertyInfo = validationContext.ObjectType.GetProperty(this.startdate);
                var propertyValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);
                if ((DateTime)value > (DateTime)propertyValue)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(ErrorMessage = "End date must not be before starting date");
                }

        }
    }
}
