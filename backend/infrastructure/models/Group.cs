using System.ComponentModel.DataAnnotations;

namespace api.models;

public class Group
{
    [Required(ErrorMessage = "Id is required")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Group name is required")]
    [StringLength(50, ErrorMessage = "Group name is too long (max 50 characters)")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Group description is required")]
    [StringLength(200, ErrorMessage = "Group description is too long (max 200 characters)")]
    public required string Description { get; set; }

    [Required(ErrorMessage = "Group image URL is required")]
    [StringLength(100, ErrorMessage = "Group image URL is too long (max 100 characters)")]
    public required string ImageUrl { get; set; }

    [Required(ErrorMessage = "Created date is required")]
    public DateTime CreatedDate { get; set; }
}

public class UpdateGroupModel
{
    [Required(ErrorMessage = "Group name is required")]
    [StringLength(50, ErrorMessage = "Group name is too long (max 50 characters)")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Group description is required")]
    [StringLength(200, ErrorMessage = "Group description is too long (max 200 characters)")]
    public required string Description { get; set; }
}

public class CreateGroupModel : UpdateGroupModel
{
    [Required(ErrorMessage = "Created date is required")]
    public DateTime CreatedDate { get; set; }
}

public class GroupCardModel : Group
{
    public decimal Amount { get; set; }
}

public class GroupInfo
{
    public int GroupId { get; set; }
    public string Name { get; set; }
}