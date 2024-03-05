using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Exceptions;
using GOTEX.Core.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace GOTEX.Core.DAL
{
    public class InspectionRejectionRepository : IRepository<InspectorRejection>
    {
        private readonly AppDbContext _context;

        public InspectionRejectionRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool DeleteRange(List<InspectorRejection> items)
        {
            throw new System.NotImplementedException();
        }

        public InspectorRejection FindById(int id) => _context.InspectorRejections.FirstOrDefault(x => x.Equals(id));

        public List<InspectorRejection> GetAll() => _context.InspectorRejections.ToList();
        public List<InspectorRejection> GetListByUserId(string id) => _context.InspectorRejections.Where(x => x.InspectorId == id).ToList();

        public InspectorRejection Insert(InspectorRejection item)
        {
            var found = _context.InspectorRejections.FirstOrDefault(x => x.InspectorId.Equals(item.InspectorId));

            if (found != null) throw new ConflictException($"InspectorRejection with Id={item.Id} already exist.");
            else
            {
                var newItem = _context.InspectorRejections.Add(item);
                _context.SaveChanges();
                return newItem.Entity;
            }
        }

        public void Remove(int id)
        {
            var found = _context.InspectorRejections.FirstOrDefault(x => x.Id == id) ?? throw new NotFoundException($"Could not find InspectorRejection with Id={id}.");

            _context.InspectorRejections.Remove(found);
            _context.SaveChanges();
        }

        public bool SendApplicationMessage(Application application, string webrootpath, List<MailTemplate> mailTemplates, Dictionary<string, string> mailSettings)
        {
            throw new System.NotImplementedException();
        }

        public string SendApplicationSubmittedMail(Application application, Dictionary<string, string> mailsettings, string webrootpath)
        {
            throw new System.NotImplementedException();
        }

        public InspectorRejection Update(InspectorRejection item)
        {
            throw new System.NotImplementedException();
        }
    }
}
