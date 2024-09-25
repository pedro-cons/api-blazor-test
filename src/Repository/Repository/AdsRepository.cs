using Domain.Entities;
using Domain.Repository;
using Repository.Context;

namespace Repository.Repository;
public class AdsRepository : BaseRepository<Ad>, IAdsRepository
{
    public AdsRepository(AppDbContext context) : base(context) { }
}
