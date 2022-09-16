namespace Catalog.Auth.Infrastructure
{
    public class ArgonOptions
    {
        public const string Argon = "argon";
        public int DegreeOfParallelism { get; set; }
        public int Iterations { get; set; }
        public int MemorySize { get; set; }
    }
}
