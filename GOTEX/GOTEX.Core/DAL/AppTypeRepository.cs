using System.Collections.Generic;
using System.Linq;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;

namespace GOTEX.Core.DAL
{
    public class AppTypeRepository : IRepository<ApplicationType>
    {
        public readonly AppDbContext _context;
        public AppTypeRepository(AppDbContext context)
        {
            _context = context;
        }
        public ApplicationType Insert(ApplicationType item)
        {
            _context.ApplicationTypes.Add(item);
            _context.SaveChanges();
            return item;
        }

        public ApplicationType Update(ApplicationType item)
        {
            _context.ApplicationTypes.Update(item);
            _context.SaveChanges();
            return item;
        }

        public void Remove(int id)
        {
            throw new System.NotImplementedException();
        }

        public ApplicationType FindById(int id)
        {
            throw new System.NotImplementedException();
        }

        public List<ApplicationType> GetAll() => _context.ApplicationTypes.ToList();

        public List<ApplicationType> GetListByUserId(string id)
        {
            throw new System.NotImplementedException();
        }

        public bool SendApplicationMessage(Application application, string webrootpath, List<MailTemplate> mailTemplates, Dictionary<string, string> mailSettings)
        {
            throw new System.NotImplementedException();
        }

        public string SendApplicationSubmittedMail(Application application, Dictionary<string, string> mailsettings, string webrootpath)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteRange(List<ApplicationType> items)
        {
            throw new System.NotImplementedException();
        }
    }
}