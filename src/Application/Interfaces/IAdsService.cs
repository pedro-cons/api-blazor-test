using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAdsService
    {
        Task<IEnumerable<AdDTO>> GetAllAsync();
        Task<ResponseDTO> AddOrUpdateAsync(AdDTO entity);
    }
}
