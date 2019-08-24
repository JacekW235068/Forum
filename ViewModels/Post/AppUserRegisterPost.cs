using Forum.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.ViewModels
{

    public class AppUserRegisterPost
    {
        [Required]
        [UserName]
        public string Username { get; set; }
        [Password]
        [Required]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public static explicit operator AppUser(AppUserRegisterPost user)
        {
            return new AppUser(user);
        }
    }
}
public class UserNameAttribute : ValidationAttribute
{
    public UserNameAttribute()
    {

    }
    protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
    {
        var v = value as string;
        if (v != null)
        {
            if (v.Length > 5)
                return ValidationResult.Success;
        }
        return new ValidationResult("Too short");
    }
}