using System.ComponentModel.DataAnnotations;

namespace infrastructure.models;

public class LoginModel
{
    [Required, EmailAddress] public string Email { get; set; }
    [Required, MinLength(8)] public string Password { get; set; }
}