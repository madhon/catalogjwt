namespace Catalog.Auth.Infrastructure;

using System.ComponentModel.DataAnnotations;

internal sealed class PersistenceOptions
{
    public const string SectionName = "ConnectionStrings";

    [Required, MinLength(5), Display(Name = "DB Connection String")]
    public string AuthDb { get; set; } = null!;

    public bool EnableDetailedErrors { get; set; }
    public bool EnableSensitiveDataLogging { get; set; }
}