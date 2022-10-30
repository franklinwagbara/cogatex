using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using GOTEX.Core.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GOTEX.Core.DAL
{
    public class ApplicationRepository : IApplication<Application>
    {
        private readonly AppDbContext _context;
        private IElpsRepository _elps;
        private IAppConfiguration<Configuration> _appConfig;
        private IAppHistory<ApplicationHistory> _history;
        private IRepository<Message> _message;
        private readonly ConstantDocument _constdocs;
        private readonly RemitaPartners _remPartners;
        private IRepository<Log> _log;
        
        public ApplicationRepository(
            AppDbContext context, 
            IElpsRepository elps,
            IAppConfiguration<Configuration> appConfig,
            IAppHistory<ApplicationHistory> history,
            IRepository<Message> message,
            IOptionsMonitor<ConstantDocument> optionsMonitor,
            IOptionsMonitor<RemitaPartners> rempartners,
            IRepository<Log> log)
        {
            _context = context;
            _elps = elps;
            _appConfig = appConfig;
            _history = history;
            _message = message;
            _constdocs = optionsMonitor.CurrentValue;
            _remPartners = rempartners.CurrentValue;
            _log = log;
        }
        public int GetCompanyElpsId(string username) 
            => _context.Users.Include("Company").FirstOrDefault(x => x.Email == username).Company.ElpsId;
        public List<Application> GetSameQuarterApplication(string userid, int quarterid, int productid) => _context.Applications
            .Include(x => x.ApplicationType)
            .Where(x => x.UserId == userid 
                        && x.QuarterId == quarterid 
                        && x.ProductId == productid
                        && x.Year.Equals(DateTime.Now.Year))
            .ToList();
        public Application Insert(Application item, bool latePayment)
        {
            decimal fee = 0.00m;
            try
            {
                var apptype = _context.ApplicationTypes.FirstOrDefault(x => x.Id == item.ApplicationTypeId);
                int la = latePayment && item.ApplicationTypeId == 2 ? 1000 : 0;
                var feeDescription = $"Statutory Fee: ${fee.ToString("N2")} {Environment.NewLine}";
                feeDescription += latePayment && item.ApplicationTypeId == 2
                    ? $"{Environment.NewLine} This is a Late Submission and so you are Expected to pay $2000.00, {Environment.NewLine} ie $1000.00 Statutory fee and $1000.00 for late Processing fee | Late Submission."
                    : "";
                item.Reference = Utils.RefrenceCode();
                item.Status = ApplicationStatus.NotSubmitted;
                item.ServiceCharge = apptype.ServiceCharge;
                item.Fee = apptype.Amount + la;
                item.Year = item.QuarterId == 1 && DateTime.UtcNow.Month >= 10 ? DateTime.Now.Year + 1 : DateTime.Now.Year;
                item.Description = feeDescription;
                item.StageId = 1;
                
                _context.Applications.Add(item);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "Save new application",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = item.LastAssignedUserId
                });
            }
            return item;
        }

        public bool Delete(Application item)
        {
            try
            {
                _context.Applications.Remove(item);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "Delete application",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = item.LastAssignedUserId
                });
            }

            return false;
        }

        public void InvalidatePaymentonElps(string webrootpath, Application application, string feedback, Dictionary<string, string> mailsettings)
        {
            try
            {
                var doctype = _context.ApplicationDocuments.FirstOrDefault(x =>
                    x.DocumentId == 240 && x.ApplicationId == application.Id);
                if (doctype != null)
                {
                    var apimodel = new ApplicationApiModel
                    {
                        Status = application.Status,
                        OrderId = application.Reference
                    };
                    var resp = Utils.Send(_appConfig.GetElpsUrl(), new HttpRequestMessage(HttpMethod.Put, 
                        $"/api/UpdateDocument/{doctype.DocumentId}/{application.User.Company.ElpsId}/false/{_appConfig.GetAppEmail()}/{$"{_appConfig.GetAppEmail()}{_appConfig.GetAppKey()}".GenerateSHA512()}")
                    {
                        Content = new StringContent(apimodel.Stringify(), Encoding.UTF8, "application/json")
                    }).Result;
                }
                //Send Application Returned Mail
                var message = new Message
                {
                    User = application.User,
                    Content = "Loading...",
                    Date = DateTime.UtcNow.AddHours(1),
                    Subject = "Application Returned",
                    ApplicationId = application.Id
                };
                _message.Insert(message);

                var body = Utils.ReadTextFile(webrootpath, "GeneralFormat.txt");
                var mailtype = $"{application.ApplicationType.Name} {application.Quarter.Name}";
                
                var tk = $"Your application for {mailtype} with reference: {application.Reference} on DPR COGATEX portal has been returned to you for payment related issues: " +
                         $"<br /> {feedback}. <br/> Follow the recommendations and resubmit your application for processing";
                message.Content = string.Format(body, message.Subject, tk, message.Id, DateTime.Now.Year, "https://cogatex.dpr..gov.ng");
                _message.Update(message);
                var comapnyemail = _context.Users.FirstOrDefault(x => x.Id == application.UserId);
                var roles = new[] { "Supervisor", "Officer", "Inspector", "ADGOPS", "HGMR" };
                var staff = _context.Users.Include(x 
                        => x.UserRoles).ThenInclude(x => x.Role).ToList();
                Utils.SendMail(mailsettings, comapnyemail.Email, message.Subject, message.Content, string.Join(',', staff.Where(x => roles.Contains(x.UserRoles.FirstOrDefault().Role.Name))));

            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "Post Payment Decline to Elps",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = application.LastAssignedUserId
                });
            }
        }

        public Application Update(Application item)
        {
            try
            {
                _context.Applications.Update(item);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "Update Application",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = item.LastAssignedUserId
                });
            }
            return item;
        }
        public List<Application> Report() => 
            GetAll().Where(x => x.Status.Equals(ApplicationStatus.Completed)).ToList();

        public (bool status, string hash, string message)  ValidatePaymentEvidence(Dictionary<string, string> dic)
        {
            string message = string.Empty;
            var resp = false;
            var reference = dic.GetValue("referencetype");

            //Check amount paid against number of applications
            if(reference.Equals("1") && _context.ApplicationTypes.FirstOrDefault()?.Amount * int.Parse(dic.GetValue("ApplicationQuantity")) != decimal.Parse(dic.GetValue("Amount")))
                message = "The amount paid and number of applications provided is invalid.";
            else
            {
                var paymentevidence = reference.Equals("1")
                    ? _context.PaymentEvidences.FirstOrDefault(x => x.ReferenceCode.Equals(dic.GetValue("referencecode")))
                    : _context.PaymentEvidences.FirstOrDefault(x => x.HashCode.Equals(dic.GetValue("referencecode")));

                if (reference.Equals("1") && paymentevidence == null)
                {
                    paymentevidence = dic.Stringify().Parse<PaymentEvidence>();
                    paymentevidence.HashCode = $"{Utils.RefrenceCode()}{dic.GetValue("referencecode")}";
                    paymentevidence.UsageCount = 1;
                    
                    _context.PaymentEvidences.Add(paymentevidence);
                    resp = true;
                    message = "Payment Evidence is valid";
                }
                else if(reference.Equals("1") && paymentevidence != null)
                    message = "Payment Evidence submitted is invalid, kindly contact Support/ICT";
                else if (!reference.Equals("1") && paymentevidence != null)
                {
                    resp = paymentevidence.ApplicationQuantity > paymentevidence.UsageCount;
                    
                    if (resp)
                    {
                        paymentevidence.UsageCount++;
                        message = "Multiple reference code is valid for this application";
                    }
                    else
                        message = "Multiple reference code is invalid";
                }
                _context.SaveChanges();
                return (resp, paymentevidence?.HashCode, message);
            }
            return (resp, string.Empty, message);
        }

        public List<Application> GetAll() => _context.Applications
            .Include("User.Company")
            .Include("ApplicationType")
            .Include("Quarter")
            .Include("Terminal")
            .Include("Product")
            .Include(x => x.Facility)
            .Include("PaymentEvidence")
            .ToList();
        public List<Application> GetListByUserId(string id) 
            => GetAll().Where(x => x.UserId == id).ToList();
        public object GetApplicationFiles(int id)
        {
            var docs = new List<DocumentType>();
            try
            {
                var application = FindById(id);
                var appType = _context.ApplicationTypes.FirstOrDefault(x => x.Id == application.ApplicationTypeId);
                var compDocs = _elps.GetCompanyDocuments(application.User.Company.ElpsId).Stringify().Parse<List<CompanyDocument>>();
                var facDocs = application.Facility == null ? new List<CompanyDocument>() : _elps.GetCompanyDocuments(application.Facility.ElpsId, "Facility").Stringify().Parse<List<CompanyDocument>>();
                var documents = _elps.GetDocumentTypes().Stringify().Parse<List<DocumentType>>();
                var facdocuments = _elps.GetDocumentTypes("Facility").Stringify().Parse<List<DocumentType>>();
                var appTypeDocs = _context.ApplicationTypeDocuments.Where(x => x.ApplicationTypeId == appType.Id).ToList();

                foreach (var item in appTypeDocs)
                {
                    if(item.DocType.ToLower().Equals("company"))
                        docs.AddRange(GetRequiredDocs(item.DocumentTypeId, application.Id, compDocs, documents).Stringify()
                            .Parse<List<DocumentType>>());
                    else
                        docs.AddRange(GetRequiredDocs(item.DocumentTypeId, application.Id, facDocs, facdocuments).Stringify()
                            .Parse<List<DocumentType>>());
                }

                if (application.Fee == 2000)
                {
                    var lateApplicationDoc = documents.FirstOrDefault(a => a.Name == "Late Submission Form");
                    if (lateApplicationDoc != null)
                    {
                        var otherdocs = GetRequiredDocs(lateApplicationDoc.Document_Id, id, compDocs, documents).Stringify()
                            .Parse<List<DocumentType>>();
                        docs.AddRange(otherdocs);
                    }
                        
                }
                
                #region Get Extra Application Documents 
                
                #endregion
            }
            catch (Exception ex)
            {
                
            }
            return docs;
        }
        public Application FindById(int id) 
            => GetAll().FirstOrDefault(x => x.Id == id);
        public Application FindByReference(string refno) 
            => GetAll().FirstOrDefault(x => x.Reference.Equals(refno));
        public List<Application> GetCompanyApplications(string email) 
            => GetAll().Where(x => x.User.Email == email).ToList();
        public object GetRequiredDocs(int docId, int applicationId, List<CompanyDocument> companyDocs, List<DocumentType> allDocs)
        {
            var application = FindById(applicationId);
            List<DocumentType> docs = new List<DocumentType>();
            var constantDocs = new List<DocumentType>();
            try
            {
                var applicationDocs = _context.ApplicationDocuments.Where(x => x.ApplicationId == applicationId).ToList();
                var docItem = allDocs.FirstOrDefault(x => x.Id == docId);
                var configDocs = _constdocs.ConstantDocuments.Split(';').Stringify().Parse<string[]>();
                
                foreach (var name in configDocs)
                {
                    var ret = allDocs.FirstOrDefault(x => x.Name == name.Trim());
                    if(ret != null)
                        constantDocs.Add(ret);
                }

                var et = constantDocs.FirstOrDefault(x => x.Id == docId);
                var addedDocuments = applicationDocs.FirstOrDefault(x => x.ApplicationTypeDocumentId == docId);
                if (addedDocuments == null)
                {
                    var existingdocs = companyDocs
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault(y => y.Document_Type_Id == docId && !y.Archived);
                    if (existingdocs == null)
                    {
                        if(docItem != null)
                            docs.Add(docItem);
                    }
                    else
                    {
                        _context.ApplicationDocuments.Add(new ApplicationDocument
                        {
                            ApplicationId = applicationId,
                            DocumentId = existingdocs.Id,
                            ApplicationTypeDocumentId = existingdocs.Document_Type_Id,
                            DocTypeId = docId
                        });
                        _context.SaveChanges();

                        docItem.Selected = true;
                        docItem.Document_Id = existingdocs.Id;
                        docItem.CoyFileId = existingdocs.Id;
                        docItem.Source = existingdocs.Source;
                        docItem.ParentSelected = existingdocs.Status;
                        docs.Add(docItem);
                    
                    }
                }
                else
                {
                    var existingdocs = companyDocs.OrderByDescending(x => x.Id)
                        .FirstOrDefault(y => y.Document_Type_Id == docId && y.Id == addedDocuments.DocumentId && !y.Archived);
                    if (existingdocs != null)
                    {
                        if (addedDocuments != null && addedDocuments.IsUploaded)
                        {
                            docItem.Selected = true;
                        }
                        else
                        {
                            docItem.Selected = true;

                            if (addedDocuments.Id != existingdocs.Id)
                            {
                                addedDocuments.DocumentId = existingdocs.Id;
                                _context.ApplicationDocuments.Update(addedDocuments);
                                _context.SaveChanges();
                            }
                        }

                        docItem.Document_Id = existingdocs.Id;
                        docItem.CoyFileId = existingdocs.Id;
                        docItem.Source = existingdocs.Source;
                        docItem.ParentSelected = existingdocs.Status;
                        docs.Add(docItem);
                    }
                    else
                    {
                        if(docItem != null)
                            docs.Add(docItem);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "Get Ruired documnents fro Application type",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = application.LastAssignedUserId
                });
            }

            return docs;
        }
        public bool UpdateApplicationDoc(int applicationid, int docTypeId, int newDocId)
        {
            var application = FindById(applicationid);
            try
            {
                var appdoc = _context.ApplicationDocuments.FirstOrDefault(x =>
                    x.ApplicationId == applicationid && x.ApplicationTypeDocumentId == docTypeId);
                if (appdoc != null)
                {
                    appdoc.IsUploaded = true;
                    appdoc.DocumentId = newDocId;

                    _context.ApplicationDocuments.Update(appdoc);
                    _context.SaveChanges();
                }
                else
                {
                    appdoc = new ApplicationDocument
                    {
                        ApplicationId = applicationid,
                        DocumentId = newDocId,
                        IsUploaded = true,
                        ApplicationTypeDocumentId = docTypeId,
                        DocTypeId = docTypeId
                    };
                    _context.ApplicationDocuments.Add(appdoc);
                    _context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "Update Application Documents",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = application.LastAssignedUserId
                });
            }
            return false;
        }
        public object ConfirmPayment(Application application, string appUrl, out bool res)
        {
            res = false;
            var remita = _context.RemitaPayments.FirstOrDefault(x => x.OrderId == application.Reference);
            try
            {
                // if (remita != null && !string.IsNullOrEmpty(remita.RRR))
                // {
                //     if (remita.RRR.ToLower().Equals("dpr-elps"))
                //     {
                //         application.Submitted = true;
                //         if (application.Status == ApplicationStatus.PaymentNotSatisfied ||
                //             application.Status == ApplicationStatus.NotSubmitted)
                //             application.Status = ApplicationStatus.PaymentPending;
                //         Update(application);
                //     }  
                // }
                // else
                // {
                //     double fee = Convert.ToDouble(application.ServiceCharge + application.Fee);
                //     //Create Invoice before redirecting
                //     var invoice = _context.Invoices.FirstOrDefault(x => x.ApplicationId == application.Id) ??
                //                   InitiateInvoice(application, fee);
                //
                //     var remitaConfigVal =
                //         _remPartners.Stringify().Parse<Dictionary<string, string>>() ?? new Dictionary<string, string>();
                //
                //     var remitaPayload = GetRemitaSplit(application, appUrl, remitaConfigVal);
                //     var applicationItems = new List<ApplicationItem>
                //     {
                //         new ApplicationItem {Group = "Export Permit", Name = application.ApplicationType.Name},
                //         new ApplicationItem { Group = "Export Permit", Name = $"Payment Description: {application.Description}" }
                //     };
                //     remitaPayload.ApplicationItems = applicationItems;
                //     var elpspayment = ElpsPostPayment(application.User.Company.ElpsId, remitaPayload).Stringify().Parse<PrePaymentResponse>();
                //
                //     remita = RecordRemitaTransaction(remita, fee, elpspayment, application);
                // }
                res = _history.CreateNextProcessingPhase(application, "SubmitPayment");
            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "Save payment confirmation",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = application.LastAssignedUserId
                });
            }
            return remita;
        }
        private RemitaPayment RecordRemitaTransaction(RemitaPayment remita, double fee, PrePaymentResponse elpspayment, Application application, string paymentYpe =  "Online")
        {
            if (remita == null)
            {
                remita = new RemitaPayment
                {
                    ApprovedAmount = fee.ToString(CultureInfo.InvariantCulture),
                    UserId = application.UserId,
                    CustomerName = application.User.Company.Name,
                    OnlineReference = elpspayment.RRR,
                    OrderId = application.Reference,
                    PaymentReference = elpspayment.RRR,
                    PaymentSource = "Remita",
                    ReferenceNumber = application.Reference,
                    ResponseCode = elpspayment.Status,
                    ResponseDescription = elpspayment.StatusMessage,
                    RRR = elpspayment.RRR,
                    TransactionAmount = fee.ToString(CultureInfo.InvariantCulture),
                    TransactionCurrency = "566",
                    TransactionDate = string.IsNullOrEmpty(elpspayment.Transactiontime) 
                        ? DateTime.UtcNow.AddHours(1).ToString(CultureInfo.InvariantCulture) : elpspayment.Transactiontime,
                    Type = paymentYpe
                };
                _context.RemitaPayments.Add(remita);
            }
            else
            {
                remita.ApprovedAmount = fee.ToString(CultureInfo.InvariantCulture);
                remita.UserId = application.UserId;
                remita.CustomerName = application.User.Company.Name;
                remita.OnlineReference = elpspayment.RRR;
                remita.OrderId = application.Reference;
                remita.PaymentReference = elpspayment.RRR;
                remita.PaymentSource = "Remita";
                remita.ReferenceNumber = application.Reference;
                remita.ResponseCode = elpspayment.Status;
                remita.ResponseDescription = elpspayment.StatusMessage;
                remita.RRR = elpspayment.RRR;
                remita.TransactionAmount = fee.ToString(CultureInfo.InvariantCulture);
                remita.TransactionCurrency = "566";
                remita.TransactionDate = string.IsNullOrEmpty(elpspayment.Transactiontime)
                    ? DateTime.UtcNow.AddHours(1).ToString(CultureInfo.InvariantCulture)
                    : elpspayment.Transactiontime;
                remita.Type = paymentYpe;
            }
            _context.SaveChanges();

            return remita;
        }
        private Invoice InitiateInvoice(Application application, double fee)
        {
            var invoice = new Invoice
            {
                Amount = fee,
                ApplicationId = application.Id,
                PaymentCode = application.Reference,
                PaymentType = "",
                Status = fee > 0 ? "Unpaid" : "Paid",
                DateAdded = DateTime.UtcNow.AddHours(1),
                DatePaid = fee > 0 ? DateTime.UtcNow.AddHours(1).AddDays(-7) : DateTime.UtcNow.AddHours(1)
            };
            _context.Invoices.Add(invoice);
            _context.SaveChanges();
            
            return invoice;
        }
        private RemitaSplit GetRemitaSplit(Application application, string appUrl, Dictionary<string, string> remitaConfigVal)
        {
            var remitaPayload = new RemitaSplit
            {
                payerPhone = application.User.PhoneNumber,
                orderId = application.Reference,
                CategoryName = "Export Permit",
                payerEmail = application.User.Email,
                payerName = application.User.Company.Name,
                AmountDue = "0",
                ReturnBankPaymentUrl = $"{appUrl}/Application/RemitaPayment",
                ReturnFailureUrl = $"{appUrl}/Application/RemitaPayment",
                ReturnSuccessUrl = $"{appUrl}/Application/Remita",
                ServiceCharge = application.ServiceCharge.ToString(),
                totalAmount = "0",
                lineItems = new List<RPartner>
                {
                    new RPartner
                    {
                        lineItemsId = "1",
                        beneficiaryName = remitaConfigVal.GetValue("AccName_1"),
                        bankCode = remitaConfigVal.GetValue("AccBC_1"),
                        beneficiaryAccount = remitaConfigVal.GetValue("Acc_1"),
                        beneficiaryAmount = "0",
                        deductFeeFrom = remitaConfigVal.GetValue("AccDeduct_1")
                    }
                }
            };

            return remitaPayload;
        }
        private object ElpsPostPayment(int elpsId, RemitaSplit remita)
        {
            try
            {
                var resp = Utils.Send(_appConfig.GetElpsUrl(), new HttpRequestMessage(HttpMethod.Post, $"/api/Payments/{elpsId}/{_appConfig.GetAppEmail()}/{$"{_appConfig.GetAppEmail()}{_appConfig.GetAppKey()}".GenerateSHA512()}")
                {
                    Content = new StringContent(remita.Stringify(), Encoding.UTF8, "application/json")
                }).Result;

                if (resp.IsSuccessStatusCode)
                {
                    var content = resp.Content.ReadAsStringAsync().Result;
                    content = content.Contains("jsonp(") ? content.Replace("jsonp(", "").Replace(")", "") : content;

                    return content.Parse<PrePaymentResponse>();
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        public bool UpdateList(List<Application> itemlist)
        {
            try
            {
                _context.Applications.UpdateRange(itemlist);
                _context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {

            }
            return false;
        }
    }
}