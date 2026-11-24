using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VoiceTel.Sdk;

/// <summary>
/// Minimal RFC 7578 multipart/form-data encoder — the wire format for
/// <c>POST /v2.2/faxes</c>. Bodies are assembled by hand around an explicit
/// boundary; no third-party multipart machinery is pulled in.
/// </summary>
internal static class MultipartFormData
{
    /// <summary>One file part: form-field name, file name, media type, raw bytes.</summary>
    internal sealed class FilePart
    {
        public FilePart(string name, string filename, string contentType, byte[] content)
        {
            Name = name;
            Filename = filename;
            ContentType = contentType;
            Content = content;
        }

        public string Name { get; }
        public string Filename { get; }
        public string ContentType { get; }
        public byte[] Content { get; }
    }

    /// <summary>A fresh random boundary — ASCII letters, digits and dashes only,
    /// safe both in the Content-Type header and in the body.</summary>
    public static string RandomBoundary() => "voicetel-" + Guid.NewGuid().ToString("N");

    /// <summary>
    /// Encode text fields followed by file parts, closing the boundary.
    /// </summary>
    /// <returns>The full Content-Type header value (boundary included) and the body bytes.</returns>
    public static (string ContentType, byte[] Body) Build(
        IReadOnlyList<(string Name, string Value)> fields,
        IReadOnlyList<FilePart> files,
        string boundary)
    {
        using var body = new MemoryStream();
        var crlf = new byte[] { (byte)'\r', (byte)'\n' };
        var dashes = new byte[] { (byte)'-', (byte)'-' };
        var boundaryBytes = Encoding.ASCII.GetBytes(boundary);
        void Delimiter()
        {
            body.Write(dashes, 0, dashes.Length);
            body.Write(boundaryBytes, 0, boundaryBytes.Length);
            body.Write(crlf, 0, crlf.Length);
        }

        foreach (var (name, value) in fields)
        {
            Delimiter();
            var header = Encoding.ASCII.GetBytes($"Content-Disposition: form-data; name=\"{name}\"\r\n");
            body.Write(header, 0, header.Length);
            body.Write(crlf, 0, crlf.Length);
            var valueBytes = Encoding.UTF8.GetBytes(value);
            body.Write(valueBytes, 0, valueBytes.Length);
            body.Write(crlf, 0, crlf.Length);
        }

        foreach (var part in files)
        {
            Delimiter();
            var header = Encoding.ASCII.GetBytes(
                $"Content-Disposition: form-data; name=\"{part.Name}\"; filename=\"{Sanitize(part.Filename)}\"\r\n" +
                $"Content-Type: {part.ContentType}\r\n");
            body.Write(header, 0, header.Length);
            body.Write(crlf, 0, crlf.Length);
            body.Write(part.Content, 0, part.Content.Length);
            body.Write(crlf, 0, crlf.Length);
        }

        body.Write(dashes, 0, dashes.Length);
        body.Write(boundaryBytes, 0, boundaryBytes.Length);
        body.Write(dashes, 0, dashes.Length);
        body.Write(crlf, 0, crlf.Length);

        return ("multipart/form-data; boundary=" + boundary, body.ToArray());
    }

    /// <summary>Keep the Content-Disposition line valid: strip quotes,
    /// backslashes and line breaks from file names.</summary>
    private static string Sanitize(string filename)
    {
        var sb = new StringBuilder(filename.Length);
        foreach (var ch in filename)
        {
            sb.Append(ch is '"' or '\\' or '\r' or '\n' ? '_' : ch);
        }
        return sb.ToString();
    }
}
