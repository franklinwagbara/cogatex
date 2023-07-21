using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using GOTEX.Core.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace GOTEX.Core.DAL
{
    public class SurveyRepository : IRepository<Survey>
    {
        private readonly AppDbContext _context;

        public SurveyRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool DeleteRange(List<Survey> items)
        {
            throw new NotImplementedException();
        }

        public Survey FindById(int id) => _context.Surveys.Find(id);

        public List<Survey> GetAll() => _context.Surveys.ToList();

        public List<Survey> GetListByUserId(string id)
        {
            throw new NotImplementedException();
        }

        public Survey Insert(Survey item)
        {
            _context.Surveys.Add(item);
            _context.SaveChanges();
            return item;
        }

        public void Remove(int id)
        {
            var survey = FindById(id);
            if(survey != null)
            {
                _context.Surveys.Remove(survey);
                _context.SaveChanges();
            }
        }

        public bool SendApplicationMessage(Application application, string webrootpath, List<MailTemplate> mailTemplates, Dictionary<string, string> mailSettings)
        {
            throw new NotImplementedException();
        }

        public string SendApplicationSubmittedMail(Application application, Dictionary<string, string> mailsettings, string webrootpath)
        {
            var body = Utils.ReadTextFile(webrootpath, "InternalMemo.txt");
            var content = "A survey form with the details " +
                "below have been submitted for your review:" +
                $"<p><table class='table table-striped'><tr><td>Company Name</td><td>{application.User.Company.Name} ({application.User.Email})</tr>" +
                $"<tr><td>Portal Name</td><td>Gas Terminal Export applcation for {application.Quarter.Name} {DateTime.Now.Year}</td></tr>" +
                $"<tr><td colspan='2' style='align: center;'><a href='{mailsettings.GetValue("portalbase")}' class='btn btn-xs btn-info'>Click here</a>to review the form details</td></tr></table></p>";
            body = string.Format(body, $"Gas Terminal Export Survey form submission ", "", "Servicom", content, DateTime.Now.Year, "");

            Utils.SendMail(mailsettings, mailsettings.GetValue("ServicomEmail"), "NUPRC Gas Terminal Export Survey form", body);

            return "Sent";
        }

        public Survey Update(Survey item)
        {
            _context.Surveys.Update(item);
            _context.SaveChanges();
            return item;
        }
    }
}
