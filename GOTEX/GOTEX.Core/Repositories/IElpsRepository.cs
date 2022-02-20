using System.Collections.Generic;
using GOTEX.Core.BusinessObjects;

namespace GOTEX.Core.Repositories
{
    public interface IElpsRepository
    {
        Dictionary<string, string> GetCompanyDetailByEmail(string email);
        List<RegisteredAddress> GetCompanyRegAddress(int id);
        bool UpdateCompanyRegAddress(List<RegisteredAddress> model);
        object AddCompanyRegAddress(List<RegisteredAddress> model, int companyId);
        RegisteredAddress GetCompanyRegAddressById(int id);
        List<DirectorModel> GetCompanyDirectors(int id);
        object UpdateCompanyDetails(CompanyModel model, string email, bool update);
        string UpdateCompanyNameEmail(object model);
        object GetDocumentTypes(string type = null);
        object GetCompanyDocuments(int id, string type = null);
        List<MailTemplate> GetMailMessages();
        List<Staff> GetAllStaff();
        Staff GetStaff(string email);
        int PushPermitToElps(PermitAPIModel item);
        Dictionary<string, string> ChangePassword(object model, string useremail);
        int CreateFacility(object item);
    }
}