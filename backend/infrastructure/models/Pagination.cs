using System.ComponentModel.DataAnnotations;

namespace infrastructure.models;

public class Pagination
{
    [Required]
    public int CurrentPage { get; set; }
    [Required]
    public int PageSize { get; set; }
}