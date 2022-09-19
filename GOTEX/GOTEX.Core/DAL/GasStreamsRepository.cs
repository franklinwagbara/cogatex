using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GOTEX.Core.DAL
{
    internal class GasStreamsRepository : IRepository<Product>
    {
        private readonly AppDbContext _context;

        public GasStreamsRepository(AppDbContext context)
        {
            _context = context;
        }
        public bool DeleteRange(List<Product> items)
        {
            throw new NotImplementedException();
        }

        public Product FindById(int id) => GetAll().FirstOrDefault(x => x.Id == id);

        public List<Product> GetAll() => _context.Products.ToList();

        public List<Product> GetListByUserId(string id)
        {
            throw new NotImplementedException();
        }

        public Product Insert(Product item)
        {
            _context.Products.Add(item);
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

        public Product Update(Product item)
        {
            _context.Products.Update(item);
            _context.SaveChanges();

            return item;
        }
    }
}
