using System.Collections.Generic;
using System.Text;

namespace VoiceTel.Sdk;

/// <summary>
/// Lightweight query-string builder. URL-encodes keys and values exactly the
/// same way that <c>System.Uri.EscapeDataString</c> does (RFC 3986).
/// </summary>
internal sealed class QueryBuilder
{
    private readonly List<(string Key, string Value)> _pairs = new();

    public void Add(string key, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }
        _pairs.Add((key, value!));
    }

    public void AddInt(string key, int value)
    {
        if (value == 0)
        {
            return;
        }
        _pairs.Add((key, value.ToString(System.Globalization.CultureInfo.InvariantCulture)));
    }

    public bool HasAny => _pairs.Count > 0;

    public override string ToString()
    {
        if (_pairs.Count == 0)
        {
            return string.Empty;
        }
        var sb = new StringBuilder();
        var first = true;
        foreach (var (k, v) in _pairs)
        {
            if (!first)
            {
                sb.Append('&');
            }
            sb.Append(System.Uri.EscapeDataString(k));
            sb.Append('=');
            sb.Append(System.Uri.EscapeDataString(v));
            first = false;
        }
        return sb.ToString();
    }
}
