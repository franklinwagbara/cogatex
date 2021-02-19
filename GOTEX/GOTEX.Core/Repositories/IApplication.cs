using System.Collections.Generic;
using GOTEX.Core.BusinessObjects;

namespace GOTEX.Core.Repositories
{
    public interface IApplication<T> where T : class
    {
        List<T> GetAll();
        int GetCompanyElpsId(string username);
        List<T> GetSameQuarterApplication(string userid, int quarterid, int productid);
        T Insert(T item, bool latePayment);
        List<T> GetListByUserId(string id);
        List<T> GetCompanyApplications(string email);
        object GetApplicationFiles(int id);
        T FindById(int id);
        T FindByReference(string refno);
        object GetRequiredDocs(int docId, int applicationId, List<CompanyDocument> companyDocs, List<DocumentType> allDocs);
        bool UpdateApplicationDoc(int applicationid, int docTypeId, int newdocid);
        object ConfirmPayment(Application application, string appUrl, out bool res);
        void InvalidatePaymentonElps(string webrootpath, Application application, string feedback, Dictionary<string, string> mailsettings);
        T Update(T item);
        List<T> Report();
        (bool status, string hash, string message) ValidatePaymentEvidence(Dictionary<string, string> dic);
    }
}