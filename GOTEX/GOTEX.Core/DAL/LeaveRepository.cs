using System;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using System.Collections.Generic;
using System.Linq;
using GOTEX.Core.Exceptions;

namespace GOTEX.Core.DAL
{
    public class LeaveRepository : IRepository<Leave>
    {
        private readonly AppDbContext _context;
        public LeaveRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool DeleteRange(List<Leave> items)
        {
            _context.Leaves.RemoveRange(items);
            var res = _context.SaveChanges();

            if (res > 0)
                return true;
            else return false;
        }

        public Leave FindById(int id) => _context.Leaves.FirstOrDefault(x => x.Id == id);

        public List<Leave> GetAll() => _context.Leaves.ToList();

        public List<Leave> GetListByUserId(string id) => _context.Leaves.Where(x => x.StaffId == id).ToList();

        public Leave Insert(Leave item)
        {
            if (_context.Leaves.FirstOrDefault(x => x.StaffId == item.StaffId) != null)
                throw new ConflictException("A Leave application for this staff already exist!");

            var newLeave = _context.Leaves.Add(item);
            _context.SaveChanges();
            return newLeave.Entity;
        }

        public void Remove(int id)
        {
            var leave = _context.Leaves.FirstOrDefault(x => x.Id == id);
            _context.Leaves.Remove(leave);
            _context.SaveChanges(); 
        }

        public bool SendApplicationMessage(Application application, string webrootpath, List<MailTemplate> mailTemplates, Dictionary<string, string> mailSettings)
        {
            throw new NotImplementedException();
        }

        public string SendApplicationSubmittedMail(Application application, Dictionary<string, string> mailsettings, string webrootpath)
        {
            throw new NotImplementedException();
        }

        public Leave Update(Leave item)
        {
            var newLeave = _context.Leaves.Update(item);
            _context.SaveChanges();
            return newLeave.Entity;
        }
    }
}
