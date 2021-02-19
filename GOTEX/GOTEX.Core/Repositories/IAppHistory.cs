using System.Collections.Generic;
using GOTEX.Core.BusinessObjects;

namespace GOTEX.Core.Repositories
{
    public interface IAppHistory<T> where T: class
    {
        bool CreateNextProcessingPhase(Application application, string action, string comment = null);
        ApplicationUser GetNextProcessingOfficer(string rolename, string action, Application application, ApplicationUser currentuser);
        List<T> GetApplicationHistoriesById(int applicationid);
        void Update(ApplicationHistory item);
    }
}