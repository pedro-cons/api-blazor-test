using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests;
using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Repository;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class AdsServiceTests
{
    private readonly Mock<IAdsRepository> _adsRepositoryMock;
    private readonly IMapper _mapper;
    private readonly AdsService _adsService;

    public AdsServiceTests()
    {
        _adsRepositoryMock = new Mock<IAdsRepository>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<AdDTO, Ad>();
            cfg.CreateMap<Ad, AdDTO>();
        });

        _mapper = config.CreateMapper();
        _adsService = new AdsService(_adsRepositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task AddOrUpdateAsync_ValidAd_AddsAd()
    {
        var adDto = new AdDTO { AdDescription = "Valid description", AdId = 0 };
        var response = Task.CompletedTask;
        _adsRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Ad>())).Returns(response);

        var result = await _adsService.AddOrUpdateAsync(adDto);

        Assert.True(result.Sucess);
        _adsRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Ad>()), Times.Once);
    }

    [Fact]
    public async Task AddOrUpdateAsync_ValidAd_UpdateAd()
    {
        var adDto = new AdDTO { AdDescription = "Valid description", AdId = 1 };
        var response = Task.CompletedTask;

        _adsRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Ad>())).Returns(response);

        var result = await _adsService.AddOrUpdateAsync(adDto);

        Assert.True(result.Sucess);
        _adsRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Ad>()), Times.Once);
    }

    [Fact]
    public async Task AddOrUpdateAsync_InvalidAd_ReturnsError()
    {
        var adDto = new AdDTO { AdDescription = "", AdId = 0 };

        var result = await _adsService.AddOrUpdateAsync(adDto);

        Assert.False(result.Sucess);
        Assert.Contains(result.Error, e => e == "\"Description\" field is mandatory.");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllAds()
    {
        var ads = new List<Ad>
        {
            new Ad { AdDescription = "Ad 1" },
            new Ad { AdDescription = "Ad 2" }
        };

        _adsRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(ads);

        var result = await _adsService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ThrowsException()
    {
        _adsRepositoryMock.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        await Assert.ThrowsAsync<Exception>(() => _adsService.GetAllAsync());
    }
}

