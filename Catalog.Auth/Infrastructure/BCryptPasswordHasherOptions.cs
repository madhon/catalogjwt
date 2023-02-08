namespace Catalog.Auth.Infrastructure
{
    public class BCryptPasswordHasherOptions
    {
        public int WorkFactor { get; set; } = 12;
    }
}
