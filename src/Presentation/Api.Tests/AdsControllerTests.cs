using Api.Controllers;
using Application.Constants;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Api.Tests;
public class AdsControllerTests
{
    private readonly Mock<IAdsService> _adsServiceMock;
    private readonly AdsController _controller;

    public AdsControllerTests()
    {
        _adsServiceMock = new Mock<IAdsService>();
        _controller = new AdsController(_adsServiceMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOkResponse()
    {
        var ads = new List<AdDTO> { new AdDTO { 
            AdDescription = "Description",
            AdBalance = 1,
            AdCreationDate = DateTime.Now,
            AdExternalId = string.Empty,
            AdStatus = Domain.Enumerators.EAdStatus.Active,
            AdTotalLeads = 1
        } };

        _adsServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(ads);

        var result = await _controller.GetAllAsync();

        var okResult = Assert.IsType<ResponseDTO<IEnumerable<AdDTO>>>(result);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        Assert.Equal(ads, okResult.Data);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnInternalServerError_OnException()
    {
        _adsServiceMock.Setup(service => service.GetAllAsync()).ThrowsAsync(new System.Exception());

        var result = await _controller.GetAllAsync();

        var errorResult = Assert.IsType<ResponseDTO<IEnumerable<AdDTO>>>(result);
        Assert.Equal((int)HttpStatusCode.InternalServerError, errorResult.StatusCode);
        Assert.Contains(CErrorMessage.ERROR_PROCESSING_REQUEST, errorResult.Error);
    }

    [Fact]
    public async Task AddorUpdateAsync_ShouldReturnOkResponse()
    {
        var ad = new AdDTO
        {
            AdDescription = "Description",
            AdBalance = 1,
            AdCreationDate = DateTime.Now,
            AdExternalId = string.Empty,
            AdStatus = Domain.Enumerators.EAdStatus.Active,
            AdTotalLeads = 1
        };
        var expectedResponse = new ResponseDTO { StatusCode = (int)HttpStatusCode.OK };
        _adsServiceMock.Setup(service => service.AddOrUpdateAsync(ad)).ReturnsAsync(expectedResponse);

        var result = await _controller.AddOrUpdateAsync(ad);

        var okResult = Assert.IsType<ResponseDTO>(result);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task AddorUpdateAsync_ShouldReturnInternalServerError_OnException()
    {
        var ad = new AdDTO
        {
            AdDescription = "Description",
            AdBalance = 1,
            AdCreationDate = DateTime.Now,
            AdExternalId = string.Empty,
            AdStatus = Domain.Enumerators.EAdStatus.Active,
            AdTotalLeads = 1
        };
        _adsServiceMock.Setup(service => service.AddOrUpdateAsync(ad)).ThrowsAsync(new System.Exception());

        var result = await _controller.AddOrUpdateAsync(ad);

        var errorResult = Assert.IsType<ResponseDTO>(result);
        Assert.Equal((int)HttpStatusCode.InternalServerError, errorResult.StatusCode);
        Assert.Contains(CErrorMessage.ERROR_PROCESSING_REQUEST, errorResult.Error);
    }
}

