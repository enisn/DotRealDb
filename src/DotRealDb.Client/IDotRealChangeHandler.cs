using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DotRealDb.Client
{
    public interface IDotRealChangeHandler
    {
        Task<ObservableCollection<T>> ConnectAndTrackAsync<T>(string dbContextName, string entityName = null, uint limit = 32);

        Task StartTrackingAsync<T>(IList<T> source, string dbContextName, string entityName = default);
    }
}
