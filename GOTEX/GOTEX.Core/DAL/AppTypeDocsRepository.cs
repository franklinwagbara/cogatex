using System;
using System.Collections.Generic;
using System.Linq;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GOTEX.Core.DAL
{
    public class AppTypeDocsRepository : IApplicationTypeDocs<ApplicationTypeDocuments>
    {
        private readonly AppDbContext _context;
        
        public AppTypeDocsRepository(AppDbContext context)
        {
            _context = context;
        }
        public bool InsertDocs(ApplicationTypeDocuments item)
        {
            try
            {
                _context.ApplicationTypeDocuments.Add(item);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                
            }
            return false;
        }

        public List<ApplicationTypeDocuments> GetAll() =>
            _context.ApplicationTypeDocuments.Include("ApplicationType").ToList();

        public bool Delete(ApplicationTypeDocuments item)
        {
            _context.ApplicationTypeDocuments.Remove(item);
            _context.SaveChanges();
            return true;
        }
    }
}