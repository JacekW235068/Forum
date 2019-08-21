using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.ViewModels
{
    //TODO: Update Requirements
    public class AppUserLoginPost
    {
        [Required]
        [Password]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}

public class PasswordAttribute : ValidationAttribute
{
    public PasswordAttribute()
    {

    }
    protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
    {
        var v = value as string;
        if(v != null)
        {
            if (v.Any(char.IsDigit))
                return ValidationResult.Success;
        }
        return new ValidationResult("Should contain a digit");
    }
}