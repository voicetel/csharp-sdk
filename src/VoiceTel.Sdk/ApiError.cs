using System;

namespace VoiceTel.Sdk;

/// <summary>
/// Classifies a VoiceTel API error so callers can switch on it without
/// having to inspect HTTP status codes.
/// </summary>
public enum ErrorKind
{
    /// <summary>Catch-all for unmapped statuses or transport failures.</summary>
    Unknown = 0,

    /// <summary>HTTP 400 — server-side validation failure.</summary>
    BadRequest,

    /// <summary>HTTP 401 — bearer token missing, expired, or invalid.</summary>
    Authentication,

    /// <summary>HTTP 403 — authenticated but not allowed.</summary>
    PermissionDenied,

    /// <summary>HTTP 404 — resource does not exist.</summary>
    NotFound,

    /// <summary>HTTP 409 — request conflicts with current state.</summary>
    Conflict,

    /// <summary>HTTP 429 — exceeded the 6/hour/IP cap on account/* endpoints.</summary>
    RateLimit,

    /// <summary>Any HTTP 5xx response.</summary>
    Server,
}

/// <summary>
/// Thrown whenever the VoiceTel API responds with a non-2xx status or when the
/// transport itself fails. <see cref="Body"/> preserves the raw response payload
/// so callers can inspect structured 409 conflict details.
/// </summary>
public sealed class ApiError : Exception
{
    /// <summary>Coarse classification of the error.</summary>
    public ErrorKind Kind { get; }

    /// <summary>HTTP status code; 0 when the failure was transport-level.</summary>
    public int StatusCode { get; }

    /// <summary>Server-supplied error code (when present in the response body).</summary>
    public string? Code { get; }

    /// <summary>
    /// The raw response body as a string, or the parsed object when the body
    /// was JSON. Useful for 409 conflicts that include partial-success detail.
    /// </summary>
    public object? Body { get; }

    /// <summary>Creates a new <see cref="ApiError"/>.</summary>
    public ApiError(
        ErrorKind kind,
        int statusCode,
        string message,
        string? code = null,
        object? body = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Kind = kind;
        StatusCode = statusCode;
        Code = code;
        Body = body;
    }

    /// <summary>True when this error has <see cref="ErrorKind.RateLimit"/>.</summary>
    public bool IsRateLimit => Kind == ErrorKind.RateLimit;

    /// <summary>True when this error has <see cref="ErrorKind.NotFound"/>.</summary>
    public bool IsNotFound => Kind == ErrorKind.NotFound;

    /// <summary>True when this error has <see cref="ErrorKind.Authentication"/>.</summary>
    public bool IsAuthentication => Kind == ErrorKind.Authentication;

    /// <summary>True when this error has <see cref="ErrorKind.Conflict"/>.</summary>
    public bool IsConflict => Kind == ErrorKind.Conflict;

    internal static ErrorKind KindFromStatus(int status) => status switch
    {
        400 => ErrorKind.BadRequest,
        401 => ErrorKind.Authentication,
        403 => ErrorKind.PermissionDenied,
        404 => ErrorKind.NotFound,
        409 => ErrorKind.Conflict,
        429 => ErrorKind.RateLimit,
        >= 500 and < 600 => ErrorKind.Server,
        _ => ErrorKind.Unknown,
    };

    internal static ApiError FromStatus(int status, string? code, string message, object? body) =>
        new(KindFromStatus(status), status, message, code, body);
}
