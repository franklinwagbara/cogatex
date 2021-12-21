using System.Collections.Generic;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;

namespace GOTEX.Core.DAL
{
    public class Logrepository : IRepository<Log>
    {
        private readonly AppDbContext _context;

        public Logrepository(AppDbContext context)
        {
            _context = context;
        }
        public Log Insert(Log item)
        {
            _context.Logs.Add(item);
            _context.SaveChanges();
            return item;
        }

        public Log Update(Log item)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(int id)
        {
            throw new System.NotImplementedException();
        }

        public Log FindById(int id)
        {
            throw new System.NotImplementedException();
        }

        public List<Log> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public List<Log> GetListByUserId(string id)
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

        public bool DeleteRange(List<Log> items)
        {
            throw new System.NotImplementedException();
        }
    }
}