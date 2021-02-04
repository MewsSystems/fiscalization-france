namespace Mews.Fiscalization.SignatureChecker.Dto
{
    internal sealed class ArchiveEntry
    {
        public ArchiveEntry(string name, string content)
        {
            Name = name;
            Content = content;
        }

        public string Name { get; }

        public string Content { get; }
    }
}