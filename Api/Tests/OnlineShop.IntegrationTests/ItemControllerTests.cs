using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using System.Text;

namespace OnlineShop.IntegrationTests;

public class ItemControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ItemControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    private StringContent GetPayload(object obj)
    {
        return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkResponse()
    {
        // Act
        var response = await _client.GetAsync("/Item");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
    }
}
