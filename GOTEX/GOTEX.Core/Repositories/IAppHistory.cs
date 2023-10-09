using System.Collections.Generic;
using System.Threading.Tasks;
using GOTEX.Core.BusinessObjects;

namespace GOTEX.Core.Repositories
{
    public interface IAppHistory<T> where T: class
    {
        Task<bool> CreateNextProcessingPhase(Application application, string action, string comment = null, string loggedinUser = null);
        Task<ApplicationUser> GetNextProcessingOfficer(string rolename, string action, Application application, ApplicationUser currentuser);
        List<T> GetApplicationHistoriesById(int applicationid);
        void Update(ApplicationHistory item);
        List<T> SentApplications(string email);
        List<T> All();
        bool UpdateList(List<T> itemList);
    }
}