using System;
using Newtonsoft.Json;

namespace Mews.Fiscalization.SignatureChecker.Dto
{
    internal sealed class ArchiveMetadata
    {
        [JsonConstructor]
        public ArchiveMetadata(string terminalIdentification, string previousRecordSignature, DateTime created, string version)
        {
            TerminalIdentification = terminalIdentification;
            PreviousRecordSignature = previousRecordSignature;
            Created = created;
            Version = version;
        }

        public string TerminalIdentification { get; }

        public string PreviousRecordSignature { get; }

        public DateTime Created { get; }

        public string Version { get; }
    }
}