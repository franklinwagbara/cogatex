using System;
using System.Collections.Generic;
using System.Linq;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using GOTEX.Core.Utilities;
using Microsoft.Extensions.Configuration;

namespace GOTEX.Core.DAL
{
    public class AppConfiguration : IAppConfiguration<Configuration>
    {
        private readonly AppDbContext _context;
        private readonly List<Configuration> _config;

        public AppConfiguration(AppDbContext context)
        {
            _context = context;
            _config = _context.Configurations.ToList();
        }

        public List<Configuration> GetConfig() => _config;
        public string GetElpsUrl() => _config.FirstOrDefault(x => x.Name.ToLower().Equals("elpsurl"))?.Value;
        public string GetAppId() => _config.FirstOrDefault(x => x.Name.ToLower().Equals("appid"))?.Value;
        public string GetAppKey() => _config.FirstOrDefault(x => x.Name.ToLower().Equals("appkey"))?.Value;
        public string GetAppEmail() => _config.FirstOrDefault(x => x.Name.ToLower().Equals("appemail"))?.Value;
        public object GetCountries() => _context.Set<Nationality>().ToList();
        public object GetApplicationTypes() => _context.Set<ApplicationType>().ToList();
        public object GetQuarters() => _context.Set<Quarter>().ToList();
        public object GetTerminal() => _context.Set<Terminal>().ToList();
        public object GetGasProducts() => _context.Set<Product>().ToList();
    }
}