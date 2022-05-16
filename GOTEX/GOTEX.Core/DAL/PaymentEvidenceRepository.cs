using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GOTEX.Core.DAL
{
    public class PaymentEvidenceRepository : IRepository<PaymentEvidence>
    {
        private readonly AppDbContext _context;

        public PaymentEvidenceRepository(AppDbContext context)
        {
            _context = context;
        }
        public PaymentEvidence Insert(PaymentEvidence item)
        {
            _context.PaymentEvidences.Add(item);
            _context.SaveChanges();
            return item;
        }

        public PaymentEvidence Update(PaymentEvidence item)
        {
            _context.PaymentEvidences.Update(item);
            _context.SaveChanges();
            return item;
        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }

        public PaymentEvidence FindById(int id) => GetAll().FirstOrDefault(x => x.Id == id);

        public List<PaymentEvidence> GetAll() => _context.PaymentEvidences.ToList();

        public List<PaymentEvidence> GetListByUserId(string id)
        {
            throw new NotImplementedException();
        }

        public bool SendApplicationMessage(Application application, string webrootpath, List<MailTemplate> mailTemplates, Dictionary<string, string> mailSettings)
        {
            throw new NotImplementedException();
        }

        public string SendApplicationSubmittedMail(Application application, Dictionary<string, string> mailsettings, string webrootpath)
        {
            throw new NotImplementedException();
        }

        public bool DeleteRange(List<PaymentEvidence> items)
        {
            throw new NotImplementedException();
        }

    }
}
