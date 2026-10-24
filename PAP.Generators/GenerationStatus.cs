namespace PAP.Generators
{
    public sealed class GenerationStatus
    {
        public string EntityName { get; internal set; }
        public string DisplayName { get; internal set; }
        public bool HasData { get; internal set; }
        public bool IsImplemented { get; internal set; }
        public bool CanGenerate { get; internal set; }
        public string StateLabel { get; internal set; }
        public string Message { get; internal set; }
    }

    public sealed class GenerationResult
    {
        public string DisplayName { get; internal set; }
        public int GeneratedCount { get; internal set; }
    }
}
