using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Interfaces;
using System.Net;
using Application.Constants;

namespace Api.Controllers;
/// <summary>
/// Controller responsible for manipulation of the ads entity
/// </summary>
[Route("Api/[controller]")]
[Produces("application/json")]
public class AdsController : BaseController
{
    #region Properties

    private readonly IAdsService adsService;

    #endregion

    #region Constructor
    public AdsController(IAdsService adsService) => this.adsService = adsService;

    #endregion

    #region Methods

    [HttpGet("[action]")]
    //[Authorize] In the scenario of needing to be authenticated
    [AllowAnonymous]
    public async Task<ResponseDTO<IEnumerable<AdDTO>>> GetAllAsync()
    {
        try
        {
            var response = new ResponseDTO<IEnumerable<AdDTO>>();

            response.Data = await adsService.GetAllAsync();
            response.StatusCode = (int)HttpStatusCode.OK;

            return response;
        }
        catch (Exception ex)
        {
            return new ResponseDTO<IEnumerable<AdDTO>>()
            {
                Error = { CErrorMessage.ERROR_PROCESSING_REQUEST },
                StatusCode = (int)HttpStatusCode.InternalServerError,
            };
        }
    }

    [HttpPost("[action]")]
    //[Authorize] In the scenario of needing to be authenticated
    [AllowAnonymous]
    public async Task<ResponseDTO> AddOrUpdateAsync([FromBody] AdDTO ad)
    {
        try
        {
            var response = await adsService.AddOrUpdateAsync(ad);

            response.StatusCode = (int)HttpStatusCode.OK;

            return response;
        }
        catch (Exception ex)
        {
            return new ResponseDTO()
            {
                Error = { CErrorMessage.ERROR_PROCESSING_REQUEST },
                StatusCode = (int)HttpStatusCode.InternalServerError,
            };
        }
    }
    #endregion
}
