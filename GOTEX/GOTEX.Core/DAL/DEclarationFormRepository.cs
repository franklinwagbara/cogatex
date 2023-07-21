using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace GOTEX.Core.DAL
{
    public class DEclarationFormRepository : IRepository<DeclarationForm>
    {
        private readonly AppDbContext _context;

        public DEclarationFormRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool DeleteRange(List<DeclarationForm> items)
        {
            throw new NotImplementedException();
        }

        public DeclarationForm FindById(int id)
        {
            throw new NotImplementedException();
        }

        public List<DeclarationForm> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<DeclarationForm> GetListByUserId(string id)
        {
            throw new NotImplementedException();
        }

        public DeclarationForm Insert(DeclarationForm item)
        {
            throw new NotImplementedException();
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

        public DeclarationForm Update(DeclarationForm item)
        {
            throw new NotImplementedException();
        }
    }
}
