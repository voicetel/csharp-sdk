using System;
using System.Threading.Tasks;
using Xunit;

namespace VoiceTel.Sdk.Tests;

/// <summary>
/// Read-only integration tests. Gated by the env vars
/// <c>VOICETEL_USERNAME</c> and <c>VOICETEL_PASSWORD</c>; skipped otherwise.
/// Run locally with:
/// <code>
///   VOICETEL_USERNAME=1000000001 VOICETEL_PASSWORD=hunter2 \
///     dotnet test --filter "Category=Integration"
/// </code>
/// </summary>
public class IntegrationTests
{
    private static (int Username, string Password, string BaseUrl)? Creds()
    {
        var u = Environment.GetEnvironmentVariable("VOICETEL_USERNAME");
        var p = Environment.GetEnvironmentVariable("VOICETEL_PASSWORD");
        if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p))
        {
            return null;
        }
        if (!int.TryParse(u, out var un))
        {
            return null;
        }
        var bu = Environment.GetEnvironmentVariable("VOICETEL_BASE_URL");
        return (un, p, string.IsNullOrEmpty(bu) ? SdkInfo.DefaultBaseUrl : bu!);
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task LoginAndReadAccountProfile()
    {
        var creds = Creds();
        if (creds is null)
        {
            return; // skipped — env vars not set
        }
        using var client = new VoiceTelClient(baseUrl: creds.Value.BaseUrl);
        await client.LoginAsync(creds.Value.Username, creds.Value.Password);
        var me = await client.Account.GetAsync();
        Assert.False(string.IsNullOrEmpty(me.Username));
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task ListNumbers_ReadOnly()
    {
        var creds = Creds();
        if (creds is null)
        {
            return;
        }
        using var client = new VoiceTelClient(baseUrl: creds.Value.BaseUrl);
        await client.LoginAsync(creds.Value.Username, creds.Value.Password);
        var numbers = await client.Numbers.ListAsync();
        Assert.NotNull(numbers);
    }
}
