namespace VoiceTel.Sdk;

/// <summary>
/// Version constants for the VoiceTel C# SDK and the API it targets.
/// </summary>
public static class SdkInfo
{
    /// <summary>This library's semantic version.</summary>
    public const string SdkVersion = "2.2.10";

    /// <summary>The VoiceTel REST API version this SDK targets.</summary>
    public const string ApiVersion = "v2.2.10";

    /// <summary>Production VoiceTel API endpoint.</summary>
    public const string DefaultBaseUrl = "https://api.voicetel.com";

    /// <summary>User-Agent header value sent on every request unless overridden.</summary>
    public const string DefaultUserAgent = "voicetel-csharp/" + SdkVersion + " (+https://github.com/voicetel/csharp-sdk)";
}
