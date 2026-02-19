using System.Net;
using System.Net.Http.Json;
using Atlas.Application.Contracts.Auth;
using FluentAssertions;
using Xunit;

namespace Atlas.Tests;

public sealed class AuthFlowTests : IClassFixture<TestAppFactory>
{
    private readonly HttpClient _client;

    public AuthFlowTests(TestAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_then_login_returns_tokens()
    {
        var email = $"user{Guid.NewGuid():N}@example.com";

        var reg = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, "SuperSecret1!"));
        reg.StatusCode.Should().Be(HttpStatusCode.OK);

        var regBody = await reg.Content.ReadFromJsonAsync<AuthResponse>();
        regBody.Should().NotBeNull();
        regBody!.AccessToken.Should().NotBeNullOrWhiteSpace();
        regBody.RefreshToken.Should().NotBeNullOrWhiteSpace();

        var login = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, "SuperSecret1!"));
        login.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginBody = await login.Content.ReadFromJsonAsync<AuthResponse>();
        loginBody.Should().NotBeNull();
        loginBody!.Email.Should().Be(email);
    }

    [Fact]
    public async Task Correlation_id_is_returned()
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "/health");
        req.Headers.Add("X-Correlation-Id", "abc123");

        var res = await _client.SendAsync(req);
        res.Headers.TryGetValues("X-Correlation-Id", out var values).Should().BeTrue();
        values!.Single().Should().Be("abc123");
    }
}
