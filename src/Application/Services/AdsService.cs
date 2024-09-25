using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Repository;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Application.Services;
public class AdsService: IAdsService
{
    #region Properties

    private readonly IAdsRepository adsRepository;
    private readonly IMapper mapper;

    #endregion

    #region Constructor

    public AdsService(IAdsRepository adsRepository, IMapper mapper) {
        this.adsRepository = adsRepository;
        this.mapper = mapper;
    }

    #endregion 

    #region Methods

    public async Task<ResponseDTO> AddOrUpdateAsync(AdDTO entity)
    {
        try
        {
            var response = isAdValid(entity);

            if (!response.Sucess)
                return response;

            var ad = mapper.Map<Ad>(entity);

            if (entity.AdId != 0)
                await adsRepository.UpdateAsync(ad);
            else
            {
                ad.AdCreationDate = DateTime.Now;
                await adsRepository.AddAsync(ad);
            }

            return response;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<AdDTO>> GetAllAsync()
    {
        try
        {
            var ads = await adsRepository.GetAllAsync();
            
            return mapper.Map<IEnumerable<AdDTO>>(ads);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private ResponseDTO isAdValid(AdDTO entity)
    {
        var response = new ResponseDTO();
        response.Error = new List<string>();

        if (string.IsNullOrEmpty(entity.AdDescription))
            response.Error.Add("\"Description\" field is mandatory.");

        if (entity.AdDescription.Length > 500)
            response.Error.Add("\"Description\" field can have a maximum of 500 characters");

        if (!string.IsNullOrEmpty(entity.AdExternalId))
        {
            if (entity.AdExternalId.Length > 500)
                response.Error.Add("\"External Id\" field can have a maximum of 500 characters");
        }

        return response;
    }
    #endregion
}
