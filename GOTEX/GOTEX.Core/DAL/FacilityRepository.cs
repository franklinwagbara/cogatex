using System.Collections.Generic;
using System.Linq;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GOTEX.Core.DAL
{
    public class FacilityRepository : IRepository<Facility>
    {
        private readonly AppDbContext _context;
        
        public FacilityRepository(AppDbContext context)
        {
            _context = context;
        }
        public Facility Insert(Facility item)
        {
            _context.Facilities.Add(item);
            _context.SaveChanges();
            return item;
        }

        public Facility Update(Facility item)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(int id)
        {
            throw new System.NotImplementedException();
        }

        public Facility FindById(int id) 
            => GetAll().FirstOrDefault(x => x.Id == id);

        public List<Facility> GetAll() => _context.Facilities
            .Include(x => x.Company)
            .Include(x => x.Terminal)
            .Include(x => x.Product).ToList(); 

        public List<Facility> GetListByUserId(string id)
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

        public bool DeleteRange(List<Facility> items)
        {
            throw new System.NotImplementedException();
        }
    }
}