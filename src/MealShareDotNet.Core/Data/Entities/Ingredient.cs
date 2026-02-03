using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MealShareDotNet.Core.Data.Entities;

[Table("Ingredient")]
public class Ingredient
{
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = String.Empty;
    public Guid ImperialUnitOfMeasure { get; set; }
    public Guid MetricUnitOfMeasure { get; set; }
}
