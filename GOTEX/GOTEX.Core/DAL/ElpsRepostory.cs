using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using GOTEX.Core.Utilities;

namespace GOTEX.Core.DAL
{
    public class ElpsRepostory : IElpsRepository
    {
        private readonly AppDbContext _context;
        private IAppConfiguration<Configuration> _appConfig;
        public ElpsRepostory(AppDbContext context, IAppConfiguration<Configuration> appConfig)
        {
            _context = context;
            _appConfig = appConfig;
        }
        public Dictionary<string, string> GetCompanyDetailByEmail(string email)
        {
            var dic = new Dictionary<string, string>();
            try
            {
                var content = CallElps($"/api/Company/{email}/", HttpMethod.Get);
                   
                if (!string.IsNullOrEmpty(content))
                    dic = content.Parse<Dictionary<string, string>>();
            }
            catch (Exception ex)
            {
                
            }
            return dic;
        }
        public RegisteredAddress GetCompanyRegAddressById(int id)
        {
            var dic = new RegisteredAddress();
            try
            {
                var content = CallElps($"/api/Address/ById/{id}/", HttpMethod.Get);
                    
                if (!string.IsNullOrEmpty(content))
                {
                    var response = content.Parse<RegisteredAddress>();
                    dic = response;
                }
            }
            catch (Exception ex)
            {
                
            }
            return dic;
        }
        public List<RegisteredAddress> GetCompanyRegAddress(int id)
        {
            var dic = new List<RegisteredAddress>();
            try
            {
                var content = CallElps($"/api/Address/{id}/", HttpMethod.Get);
                    
                if (!string.IsNullOrEmpty(content))
                {
                    var response = content.Parse<List<RegisteredAddress>>();
                    dic = response;
                }
            }
            catch (Exception ex)
            {
                
            }
            return dic;
        }
        public bool UpdateCompanyRegAddress(List<RegisteredAddress> model)
        {
            try
            {
                var content = CallElps($"/api/Address/{_appConfig.GetAppEmail()}/{HttpHash()}", HttpMethod.Put, model);
                if (!string.IsNullOrEmpty(content))
                    return true;
            }
            catch (Exception ex)
            {
                
            }
            return false;
        }
        public object AddCompanyRegAddress(List<RegisteredAddress> model, int companyId)
        {
            try
            {
                var content = CallElps($"/api/Address/{companyId}/", HttpMethod.Post, model);
                if (!string.IsNullOrEmpty(content))
                    return content.Parse<List<RegisteredAddress>>();
            }
            catch (Exception ex)
            {
                
            }
            return false;
        }
        public List<DirectorModel> GetCompanyDirectors(int id)
        {
            var dic = new List<DirectorModel>();
            try
            {
                var content = CallElps($"/api/Directors/{id}/", HttpMethod.Get);
                
                if (!string.IsNullOrEmpty(content))
                    dic = content.Parse<List<DirectorModel>>();

                //var docs = CallElps($"/api/Documents/Types/", HttpMethod.Get);
            }
            catch (Exception ex)
            {
                
            }
            return dic;
        }
        public object UpdateCompanyDetails(CompanyModel model, string email, bool update)
        {
            try
            {
                var content = CallElps("/api/Company/Edit/", HttpMethod.Put, 
                    new {
                        company = model, 
                        companyMedicals = (string)null,
                        companyExpatriateQuotas = (string)null,
                        companyNsitfs = (string)null,
                        companyProffessionals = (string)null,
                        companyTechnicalAgreements = (string)null
                    });
                if (!string.IsNullOrEmpty(content) && update)
                {
                    var company = model.Stringify().Parse<CompanyModel>();
                    if (company != null)
                    {
                        var res = UpdateCompanyNameEmail(new
                        {
                            Name = company.name,
                            RC_Number = company.rC_Number,
                            Business_Type = company.business_Type,
                            emailChange = true,
                            CompanyId = company.id,
                            NewEmail = email
                        });
                        if(!string.IsNullOrEmpty(res))
                            return company.Stringify().Parse<object>();
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            return null;
        }
        public string UpdateCompanyNameEmail(object model)
        {
            try
            {
                var content = CallElps("/api/Accounts/ChangeEmail/", HttpMethod.Post, model);
                if (!string.IsNullOrEmpty(content))
                    return content;
            }
            catch (Exception ex)
            {
                
            }
            return null;
        }
        public object GetDocumentTypes(string type = null)
        {
            var docs = new List<DocumentType>();
            try
            {
                string requestUri = string.IsNullOrEmpty(type) ? 
                    $"/api/Documents/Types/{_appConfig.GetAppEmail()}/{HttpHash()}"
                    : $"/api/Documents/Facility/{_appConfig.GetAppEmail()}/{HttpHash()}/{type}";
                var resp = Utils.Send(_appConfig.GetElpsUrl(), new HttpRequestMessage(HttpMethod.Get, requestUri))
                    .Result;
                if (resp.IsSuccessStatusCode)
                {
                    var content = resp.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(content))
                        docs = content.Parse<List<DocumentType>>();
                }
            }
            catch (Exception ex)
            {
                
            }
            return docs;
        }
        public object GetCompanyDocuments(int id, string type = null)
        {
            var docs = new List<CompanyDocument>();
            try
            {
                var requestUri = string.IsNullOrEmpty(type) || type.Trim().ToLower().Equals("company")
                    ? $"/api/CompanyDocuments/{id}/"
                    : $"/api/FacilityDocuments/{id}/";
                var content = CallElps(requestUri, HttpMethod.Get);
                if (!string.IsNullOrEmpty(content))
                    docs = content.Parse<List<CompanyDocument>>();
            }
            catch (Exception ex)
            {
                
            }
            return docs;
        }
        private string CallElps(string requestUri, HttpMethod method, object body = null)
        {
            var resp  = new HttpResponseMessage();
            if (body != null)
                resp = Utils.Send(
                    _appConfig.GetElpsUrl(),
                    new HttpRequestMessage(method, $"{requestUri}{_appConfig.GetAppEmail()}/{HttpHash()}")
                    {
                        Content = new StringContent(body.Stringify(), Encoding.UTF8, "application/json")
                    }).Result;
            else
                resp = Utils.Send(
                    _appConfig.GetElpsUrl(),
                    new HttpRequestMessage(method, $"{requestUri}{_appConfig.GetAppEmail()}/{HttpHash()}"){}).Result;
            
            if (resp.IsSuccessStatusCode)
                return resp.Content.ReadAsStringAsync().Result;
            
            return null;
        }
        private string HttpHash() => $"{_appConfig.GetAppEmail()}{_appConfig.GetAppKey()}".GenerateSHA512();
        public List<MailTemplate> GetMailMessages() => _context.MailTemplates.ToList();
        
        public List<Staff> GetAllStaff()
        {
            try
            {
                var resp = CallElps("/api/Accounts/Staff/", HttpMethod.Get);
                if (!string.IsNullOrEmpty(resp))
                    return resp.Parse<List<Staff>>();
            }
            catch (Exception ex)
            {
                
            }
            return null;
        }

        public Staff GetStaff(string email)
        {
            try
            {
                var resp = CallElps($"/api/Accounts/Staff/{email}/", HttpMethod.Get);
                if (!string.IsNullOrEmpty(resp))
                    return resp.Parse<Staff>();
            }
            catch (Exception ex)
            {
                
            }
            return null;
        }
        
        public int PushPermitToElps(PermitAPIModel item)
        {
            int elpsid = 0;
            try
            {
                var resp = CallElps($"/api/Permits/{item.Company_Id}/",
                    HttpMethod.Post, item);
                
                if (!string.IsNullOrEmpty(resp))
                {
                    var content = resp.Parse<PermitAPIModel>();
                    if (content.Id > 0)
                        elpsid = content.Id;
                }
            }
            catch(Exception ex)
            {
                
            }
            return elpsid;
        }
    }
}