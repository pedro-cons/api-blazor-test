using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using Web.Models;

namespace Web.Services;
public class AdServiceRest : IAdServiceRest
{
    #region Properties

    private readonly HttpClient _httpClient;

    #endregion

    #region Constructor
    public AdServiceRest(HttpClient httpClient) => this._httpClient = httpClient;

    #endregion

    #region Methods

    public async Task Create(AdDTO ad)
    {
        var jsonContent = new StringContent(JsonSerializer.Serialize(ad), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("Api/Ads/AddOrUpdate", jsonContent);

        response.EnsureSuccessStatusCode();
    }


    public async Task<IEnumerable<AdDTO>> GetAll()
    {
        var response = await _httpClient.GetFromJsonAsync<ResponseDTO<IEnumerable<AdDTO>>>("Api/Ads/GetAll");

        return response.Data;
    }

    #endregion
}
