using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using FuncSharp;
using Newtonsoft.Json;

namespace Mews.SignatureChecker
{
    internal class Archive
    {
        private Archive(IReadOnlyList<ArchiveEntry> entries, ArchiveMetadata metadata, byte[] signature)
        {
            Entries = entries;
            Metadata = metadata;
            Signature = signature;
        }

        public IReadOnlyList<ArchiveEntry> Entries { get; }

        public ArchiveMetadata Metadata { get; }

        public byte[] Signature { get; }

        public static Try<Archive, string> Load(string path)
        {
             return File.Exists(path).Match(
                t => ReadArchive(path),
                f => Try.Error("File does not exist.")
             );
        }

        public Try<T, string> ProcessEntry<T>(string namePrefix, Func<ArchiveEntry, T> parser)
        {
            return ProcessEntry(Entries, namePrefix, parser);
        }

        private static Try<Archive, string> ReadArchive(string path)
        {
            var entries = Try.Create(_ =>
            {
                using (var stream = File.OpenRead(path))
                using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    return zip.Entries.Select(e => ReadArchiveEntry(e)).ToList();
                }
            });
            return entries.MapError(e => "Invalid archive.").FlatMap(e =>
            {
                var metadata = GetMetadata(e);
                var signature = GetSignature(e);
                return metadata.FlatMap(m => signature.Map(s => new Archive(e, m, s)));
            });
        }

        private static ArchiveEntry ReadArchiveEntry(ZipArchiveEntry zipEntry)
        {
            using (var stream = zipEntry.Open())
            {
                var content = Encoding.UTF8.GetString(stream.ReadFully());
                return new ArchiveEntry(zipEntry.Name, content);
            }
        }

        private static Try<ArchiveMetadata, string> GetMetadata(IReadOnlyList<ArchiveEntry> archiveEntries)
        {
            return ProcessEntry(archiveEntries, "METADATA.json", e => JsonConvert.DeserializeObject<ArchiveMetadata>(e.Content)).FlatMap(m =>
            {
                var isVersionSupported = m.Version == "1.0" || m.Version == "4.0";
                return isVersionSupported.Match(
                    t => Try.Success<ArchiveMetadata, string>(m),
                    f => Try.Error("Archive version is not supported.")
                );
            });
        }

        private static Try<byte[], string> GetSignature(IReadOnlyList<ArchiveEntry> archiveEntries)
        {
            return ProcessEntry(archiveEntries, "SIGNATURE.txt", e => Base64Url.GetBytes(e.Content));
        }

        private static Try<T, string> ProcessEntry<T>(IReadOnlyList<ArchiveEntry> archiveEntries, string namePrefix, Func<ArchiveEntry, T> parser)
        {
            var entry = archiveEntries.SingleOption(e => e.Name.StartsWith(namePrefix)).Match(
                e => Try.Success<ArchiveEntry, string>(e),
                _ => Try.Error($"No unique file found {namePrefix}*.")
            );
            return entry.FlatMap(e =>
            {
                var result = Try.Create(_ => parser(e));
                return result.MapError(_ => $"Invalid data ({e.Name}).");
            });
        }
    }

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