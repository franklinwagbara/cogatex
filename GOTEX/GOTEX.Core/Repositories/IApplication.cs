using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GOTEX.Core.BusinessObjects;

namespace GOTEX.Core.Repositories
{
    public interface IApplication<T> where T : class
    {
        List<T> GetAll();
        List<T> GetStappApps(string id);
        int GetCompanyElpsId(string username);
        List<T> GetSameQuarterApplication(string userid, int quarterid, int productid);
        T Insert(T item, bool latePayment);
        bool Delete(T item);
        List<T> GetListByUserId(string id);
        List<T> GetCompanyApplications(string email);
        object GetApplicationFiles(int id);
        T FindById(int id);
        T FindByReference(string refno);
        object GetRequiredDocs(int docId, int applicationId, List<CompanyDocument> companyDocs, List<DocumentType> allDocs);
        bool UpdateApplicationDoc(int applicationid, int docTypeId, int newdocid);
        Task<object> ConfirmPayment(Application application, string appUrl, bool res);
        void InvalidatePaymentonElps(string webrootpath, Application application, string feedback, Dictionary<string, string> mailsettings);
        T Update(T item);
        List<T> Report(DateTime min, DateTime max);
        (bool status, string hash, string message) ValidatePaymentEvidence(Dictionary<string, string> dic);
        bool UpdateList(List<T> itemList);
    }
}