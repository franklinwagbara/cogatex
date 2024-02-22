using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Exceptions;
using GOTEX.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GOTEX.Core.DAL
{
    public class LeaveRequestRepository : IRepository<LeaveRequest>
    {
        private readonly AppDbContext _context;
        public LeaveRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool DeleteRange(List<LeaveRequest> items)
        {
            throw new System.NotImplementedException();
        }

        public LeaveRequest FindById(int id) => _context.LeaveRequests.FirstOrDefault(e => e.Id == id);

        public List<LeaveRequest> GetAll() => _context.LeaveRequests
            .Include(x => x.ApprovingStaff).Include(x => x.Leave).ThenInclude(x => x.Staff).Include(x => x.Leave).ThenInclude(x => x.ActingStaff).ToList();

        public List<LeaveRequest> GetListByUserId(string id)
        {
            throw new System.NotImplementedException();
        }

        public LeaveRequest Insert(LeaveRequest item)
        {
            var res = _context.LeaveRequests.Add(item);
            _context.SaveChanges();
            return res.Entity;
        }

        public void Remove(int id)
        {
            var found = _context.LeaveRequests.FirstOrDefault(_ => _.Id == id) ?? throw new NotFoundException($"LeaveRequest with Id={id} was not found.");
            _context.LeaveRequests.Remove(found);
        }

        public bool SendApplicationMessage(Application application, string webrootpath, List<MailTemplate> mailTemplates, Dictionary<string, string> mailSettings)
        {
            throw new System.NotImplementedException();
        }

        public string SendApplicationSubmittedMail(Application application, Dictionary<string, string> mailsettings, string webrootpath)
        {
            throw new System.NotImplementedException();
        }

        public LeaveRequest Update(LeaveRequest item)
        {
            throw new System.NotImplementedException();
        }
    }
}
