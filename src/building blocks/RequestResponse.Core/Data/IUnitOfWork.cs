using System.Threading.Tasks;

namespace RequestResponse.Core.Data
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();
    }
}
