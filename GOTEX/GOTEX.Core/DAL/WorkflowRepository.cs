using System.Collections.Generic;
using System.Linq;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;

namespace GOTEX.Core.DAL
{
    public class WorkflowRepository : IRepository<WorkFlow>
    {
        private readonly AppDbContext _context;
        public WorkflowRepository(AppDbContext context)
        {
            _context = context;
        }
        public WorkFlow Insert(WorkFlow item)
        {
            _context.WorkFlows.Add(item);
            _context.SaveChanges();
            return item;
        }

        public WorkFlow Update(WorkFlow item)
        {
            _context.WorkFlows.Update(item);
            _context.SaveChanges();
            return item;
        }

        public void Remove(int id)
        {
            var item = FindById(id);
            _context.WorkFlows.Remove(item);
            _context.SaveChanges();
            
        }

        public WorkFlow FindById(int id) => _context.WorkFlows.FirstOrDefault(x => x.Id == id);

        public List<WorkFlow> GetAll() => _context.WorkFlows.ToList();

        public List<WorkFlow> GetListByUserId(string id)
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

        public bool DeleteRange(List<WorkFlow> items)
        {
            throw new System.NotImplementedException();
        }
    }
}