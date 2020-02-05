using System;
using FuncSharp;
using Newtonsoft.Json;

namespace Mews.SignatureChecker
{
    internal sealed class ArchiveMetadata
    {
        [JsonConstructor]
        public ArchiveMetadata(string terminalIdentification, string previousRecordSignature, DateTime created, string version)
        {
            TerminalIdentification = terminalIdentification;
            PreviousRecordSignature = previousRecordSignature.ToNonEmptyOption();
            Created = created;
            Version = version;
        }

        public string TerminalIdentification { get; }

        public Option<string> PreviousRecordSignature { get; }

        public DateTime Created { get; }

        public string Version { get; }
    }
}