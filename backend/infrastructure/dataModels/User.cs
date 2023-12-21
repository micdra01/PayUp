using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;

namespace infrastructure.dataModels;

public class User
{
    public required int Id { get; set; }
    
    [Required] public required string FullName { get; set; }
    [Required, EmailAddress] public required string Email { get; set; }
    [Required, Phone] public required string PhoneNumber { get; set; }
    [Required, Timestamp] public required DateTime Created { get; set; }
    [Required, Url] public required string ProfileUrl { get; set; }
}

public class UserInfoDto
{
    public int Id { get; set; }
    [Required, EmailAddress] public required string Email { get; set; }
    [Required] public required string FullName { get; set; }
    [Required, Phone] public required string PhoneNumber { get; set; }
}


public class ShortUserDto
{
    public int Id { get; set; }
    [Required] public required string FullName { get; set; }
    [Required, Url] public required string ProfileUrl { get; set; }
}

public class UserInGroupDto
{
    public int UserId { get; set; }
    public int GroupId { get; set; }
    public bool IsOwner { get; set; }
}

public class InvitableUser
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string FullName { get; set; }
    [Required, Url]
    public string ProfileUrl { get; set; }
}