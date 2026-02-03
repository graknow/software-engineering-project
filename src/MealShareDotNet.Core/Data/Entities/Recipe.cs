using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MealShareDotNet.Core.Data.Entities;

[Table("Recipe")]
public class Recipe
{
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Index]
    [Required]
    public string Name { get; set; } = String.Empty;
    public int CookTime { get; set; }
    public int Price { get; set; }
    public int ServingQuantity { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastUpdatedDate { get; set; }
}
