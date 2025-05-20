using FluentAssertions;
using System.Net.Http.Json;
using TheInBazar.Service.DTOs;

namespace TheInBazar.Test;

public class ApiTests : IClassFixture<CustomWebAppFactory>
{
    private readonly HttpClient _httpClient;

    public ApiTests(CustomWebAppFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task PostAsync_ShouldUserCreate()
    {
        var newUser = new UserForCreationDto
        {
            Firstname = "Diyorbek",
            Lastname = "Juraev",
            Email = "31diy",
            Password = "31052005"
        };

        var response = await _httpClient.PostAsJsonAsync("api/users", newUser);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<Response>();
        result!.StatusCode.Should().Be(200);
        result.Message.Should().Be("Success");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnUsers()
    {
        var response = await _httpClient.GetAsync("api/users");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<Response>();
        result!.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
    }
}
