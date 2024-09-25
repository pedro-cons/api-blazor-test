using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Web.Models;
using Web.Services;
using Xunit;

namespace Web.Tests;

public class AdServiceRestTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly AdServiceRest _adService;

    public AdServiceRestTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClient.BaseAddress = new Uri("http://localhost");
        _adService = new AdServiceRest(_httpClient);
    }

    [Fact]
    public async Task Create_ShouldPostDataAndReturnSuccess()
    {
        var adDto = new AdDTO { AdDescription = "Test Ad", AdTotalLeads = 10 };
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        await _adService.Create(adDto);

        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post &&
                req.RequestUri == new Uri("http://localhost/Api/Ads/AddOrUpdate") &&
                req.Content is StringContent),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetAll_ShouldReturnListOfAds()
    {
        var adList = new List<AdDTO>
        {
            new AdDTO { AdDescription = "Test Ad 1", AdTotalLeads = 10 },
            new AdDTO { AdDescription = "Test Ad 2", AdTotalLeads = 20 }
        };

        var response = new ResponseDTO<IEnumerable<AdDTO>> { Data = adList };
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(response), Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        var result = await _adService.GetAll();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("Test Ad 1", result.First().AdDescription);
    }
}
