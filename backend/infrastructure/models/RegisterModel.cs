using System.ComponentModel.DataAnnotations;

namespace infrastructure.models;

public class RegisterModel
{
    [Required, EmailAddress] public required string Email { get; set; }

    [Required] public required string FullName { get; set; }

    [Required] [MinLength(8)] public required string Password { get; set; }

    [Required, Phone] public required string PhoneNumber { get; set; }

    [Required] public required DateTime Created { get; set; }

    [Required, Url] public required string ProfileUrl { get; set; }
}