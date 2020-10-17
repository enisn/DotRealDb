using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotRealDb.Client
{
    public interface IDotRealChangeHandler
    {
        Task StartTrackingAsync<T>(IList<T> source, string dbContextName, string entityName = default);
    }
}
