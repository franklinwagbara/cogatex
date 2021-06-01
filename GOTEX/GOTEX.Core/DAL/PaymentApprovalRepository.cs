using System;
using System.Collections.Generic;
using System.Linq;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GOTEX.Core.DAL
{
    public class PaymentApprovalRepository : IPayment<PaymentApproval>
    {
        private AppDbContext _context;
        public PaymentApprovalRepository(AppDbContext context)
        {
            _context = context;
        }
        public PaymentApproval Insert(PaymentApproval item)
        {
            _context.PaymentApprovals.Add(item);
            _context.SaveChanges();

            return item;
        }
        public string CreateManualPayment(Application application, ManualRemitaValue item, double amount, string feedback)
        {
            string desc = "";
            try
            {
                var manual = _context.ManualRemitaValues.FirstOrDefault(x => x.RRR.Contains(application.Reference));
                if (manual == null)
                    _context.ManualRemitaValues.Add(item);

                var remita = _context.RemitaPayments.FirstOrDefault(x => x.ReferenceNumber == application.Reference);
                if (remita == null)
                {
                    _context.RemitaPayments.Add(new RemitaPayment
                    {
                        ApprovedAmount = item.NetAmount.ToString(),
                        CustomerName = application.User.Company.Name,
                        OnlineReference = "DPR-Bank-M",
                        OrderId = application.Reference,
                        PaymentSource = "Bank",
                        PaymentReference = "DPR-Bank-M",
                        QueryDate = DateTime.UtcNow.AddYears(-10),
                        ReferenceNumber = application.Reference,
                        ResponseCode = "01",
                        ResponseDescription = "Payment Completed",
                        RRR = "DPR-Bank-M",
                        TransactionAmount = manual.NetAmount.ToString(),
                        TransactionCurrency = "566",
                        TransactionDate = DateTime.UtcNow.AddHours(1).ToString(),
                        Type = "Offline"
                    });
                    _context.RemitaPayments.Add(remita);
                    desc =  $" :: Amount Confirmed: {amount.ToString("N2")}, Comment: {feedback}. Done by: {application.User.Email}";

                }
                else
                {
                    remita.ResponseCode = "01";
                    remita.ResponseDescription = "Payment Completed";
                    _context.RemitaPayments.Update(remita);
                    desc = $" :: Amount Confirmed: {amount.ToString("N2")}, Comment: {feedback}. Done by: {application.User.Email}";
                }
                _context.SaveChanges();
                
                //Send ApplicationSubmitted mail, implement method

            }
            catch (Exception ex)
            {
                
            }
            return desc;
        }

        public List<PaymentApproval> GetAll() 
            => _context.PaymentApprovals
                .Include("User")
                .Include("Application.User.Company")
                .Include("Application.ApplicationType")
                .Include("Application.Quarter")
                .Include("Application.Terminal")
                .Include("Application.Product")
                .Include("Application.PaymentEvidence")
                .ToList();
    
    }
}