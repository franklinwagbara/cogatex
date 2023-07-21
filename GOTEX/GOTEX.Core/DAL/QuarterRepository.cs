using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GOTEX.Core.DAL
{
    public class QuarterRepository : IRepository<Quarter>
    {
        private readonly AppDbContext _context;

        public QuarterRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool DeleteRange(List<Quarter> items)
        {
            throw new NotImplementedException();
        }

        public Quarter FindById(int id) => _context.Quarters.FirstOrDefault(x => x.Id == id);

        public List<Quarter> GetAll() => _context.Quarters.ToList();

        public List<Quarter> GetListByUserId(string id)
        {
            throw new NotImplementedException();
        }

        public Quarter Insert(Quarter item)
        {
            _context.Quarters.Add(item);
            _context.SaveChanges();
            return item;
        }

        public void Remove(int id)
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

        public Quarter Update(Quarter item)
        {
            throw new NotImplementedException();
        }
    }
}
