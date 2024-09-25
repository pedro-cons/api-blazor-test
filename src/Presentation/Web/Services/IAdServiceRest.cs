using Web.Models;

namespace Web.Services
{
    public interface IAdServiceRest
    {
        Task Create(AdDTO ad);
        Task<IEnumerable<AdDTO>> GetAll();
    }
}
