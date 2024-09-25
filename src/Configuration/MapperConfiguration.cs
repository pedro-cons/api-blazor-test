using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Configuration;
/// <summary>
/// Class responsible for mapping objects
/// </summary>
public class MapperConfiguration : Profile
{
    public MapperConfiguration()
    {
        CreateMap<Ad, AdDTO>().ReverseMap();
    }
}
