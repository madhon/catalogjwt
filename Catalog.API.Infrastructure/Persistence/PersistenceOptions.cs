namespace Catalog.API.Infrastructure.Persistence;

using System.ComponentModel.DataAnnotations;

public class PersistenceOptions
{
    public const string SectionName = "ConnectionStrings";

    [Required, MinLength(5), Display(Name = "DB Connection String")]
    public string CatalogDb { get; set; } = null!;

    public bool EnableDetailedErrors { get; set; }
    public bool EnableSensitiveDataLogging { get; set; }
}