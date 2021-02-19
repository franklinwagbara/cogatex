using System.Collections.Generic;
using GOTEX.Core.BusinessObjects;

namespace GOTEX.Core.Repositories
{
    public interface IPermit<T> where T: class
    {
        string CreatePermit(int applicationid, string webrootpath, Dictionary<string, string> mailsettings);
        T FindByPermitNumber(string permitno);
        T FindById(int id);
        T Update(Permit permit);
        T Insert(Permit item);
        List<T> GetCompanyPermits(string userid);
        List<T> All();
    }
}