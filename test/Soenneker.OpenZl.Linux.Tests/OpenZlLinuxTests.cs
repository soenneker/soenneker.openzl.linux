using System;
using System.IO;
using System.Security.Cryptography;
namespace Soenneker.OpenZl.Linux.Tests;

public sealed class OpenZlLinuxTests
{
    [Test]
    public void NativeAssetMatchesProvenanceAndIncludesLicenses()
    {
        string directory = Path.Combine(AppContext.BaseDirectory, "Resources", "linux-x64", "native");
        byte[] bytes = File.ReadAllBytes(Path.Combine(directory, "libopenzl.so"));
        if (!bytes.AsSpan().StartsWith(new byte[] { 0x7f, 0x45, 0x4c, 0x46 })) throw new InvalidDataException("Incorrect native binary format.");
        string hash = Convert.ToHexStringLower(SHA256.HashData(bytes));
        string provenance = File.ReadAllText(Path.Combine(directory, "SOURCE.txt"));
        if (!provenance.Contains($"SHA256 (libopenzl.so): {hash}", StringComparison.Ordinal)) throw new InvalidDataException("Native binary does not match its provenance.");
        foreach (string license in new[] { "OpenZL.txt", "Zstandard.txt", "LZ4.txt" })
            if (new FileInfo(Path.Combine(directory, "licenses", license)).Length == 0) throw new InvalidDataException("Missing license content.");
    }
}
