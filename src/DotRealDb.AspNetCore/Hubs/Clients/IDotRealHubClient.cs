using System.Threading.Tasks;

namespace DotRealDb.AspNetCore.Hubs.Clients
{
    public interface IDotRealHubClient
    {
        Task Added<T>(T entity);

        Task Modified<T>(T entity);

        Task Deleted<T>(T Entity);
    }
}
