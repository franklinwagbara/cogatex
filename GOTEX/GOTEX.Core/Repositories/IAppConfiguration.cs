using System.Collections.Generic;
using GOTEX.Core.BusinessObjects;

namespace GOTEX.Core.Repositories
{
    public interface IAppConfiguration<T> where T : class
    {
        List<T> GetConfig();
        string GetElpsUrl();
        string GetAppId();
        string GetAppKey();
        string GetAppEmail();
        object GetCountries();
        object GetApplicationTypes();
        object GetQuarters();
        object GetTerminal();
        object GetGasProducts();
    }
}