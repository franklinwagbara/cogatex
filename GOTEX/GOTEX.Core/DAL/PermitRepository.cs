using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using GOTEX.Core.Utilities;
using Microsoft.EntityFrameworkCore;

namespace GOTEX.Core.DAL
{
    public class PermitRepository : IPermit<Permit>
    {
        private IApplication<Application> _application;
        private AppDbContext _context;
        private IElpsRepository _elps;
        private IRepository<Message> _message;
        
        public PermitRepository(
            IApplication<Application> application, 
            IElpsRepository elps,
            AppDbContext context,
            IRepository<Message> message)
        {
            _application = application;
            _elps = elps;
            _context = context;
            _message = message;
        }
        
        public string CreatePermit(int applicationid, string webrootpath, Dictionary<string, string> mailsettings)
        {
            try
            {
                var application = _application.FindById(applicationid);

                if (application.Status.Equals(ApplicationStatus.Completed))
                {
                    var getPm = _context.Permits.FirstOrDefault(x => x.ApplicationId == applicationid);
                    if (getPm == null)
                    {

                        var elpspermitmodel = new PermitAPIModel();
                        var permit = new Permit
                        {
                            ApplicationId = application.Id,
                            UserId = application.UserId,
                            DateIssued = DateTime.UtcNow.AddHours(1),
                            LicenseName = $"{application.ApplicationType.Name} Gas Export ({application.Quarter.Name})"
                        };
                        DateTime date = DateTime.UtcNow.Date;
                        permit = GeneratePermitExpiry(permit, date, application.Quarter.Name);

                        if (application.ApplicationType.Name.Equals("renew", StringComparison.OrdinalIgnoreCase))
                        {
                            permit.PermitNumber = GeneratePermitNo("R");
                            elpspermitmodel.Is_Renewed = "renew";

                            //Update old permit to Completed status
                            if (!string.IsNullOrEmpty(application.CurrentPermit))
                            {
                                var oldpermit = FindByPermitNumber(application.CurrentPermit);
                                if (oldpermit != null)
                                {
                                    permit.IsRenewed = "Completed";
                                    Update(permit);
                                }
                            }
                        }
                        else
                        {
                            permit.PermitNumber = GeneratePermitNo("N");
                            elpspermitmodel.Is_Renewed = "new";
                        }

                        elpspermitmodel.CategoryName = "Gas Export Permit";
                        elpspermitmodel.Company_Id = application.User.Company.ElpsId;
                        elpspermitmodel.Date_Expire = permit.ExpiryDate;
                        elpspermitmodel.Date_Issued = permit.DateIssued;
                        elpspermitmodel.OrderId = application.Reference;
                        elpspermitmodel.Permit_No = permit.PermitNumber;

                        permit.ElpsId = _elps.PushPermitToElps(elpspermitmodel);
                        Insert(permit);

                        //Send Mail
                        var dt = date.Day.ToString() + date.Month.ToString() + date.Year.ToString();
                        var sn = string.Format("DPR/GATEX/{0}/{1}", dt, application.User.Company.Id);
                        var body = Utils.ReadTextFile(webrootpath, "NDTs.txt");
                        string subject = "Export Permit Approval for your Company";
                        var msgBody = string.Format(body, subject, application.User.Company.Name,
                            permit.PermitNumber, permit.DateIssued.ToLongDateString(),
                            permit.ExpiryDate.ToLongDateString(),
                            $"{application.Quarter.Name} for {application.Quantity.ToString("N2")} " +
                            $"Barrels of {application.Product.Name} ", DateTime.Now.Year,
                            application.Quarter.Name);

                        Utils.SendMail(mailsettings, application.User.Email, subject, msgBody);

                        _message.Insert(new Message
                        {
                            Content = msgBody,
                            Date = DateTime.UtcNow.AddHours(1),
                            Subject = subject,
                            User = application.User,
                            ApplicationId = application.Id
                        });

                        return permit.PermitNumber;
                    }
                    else
                        return getPm.PermitNumber;
                }
            }
            catch (Exception ex)
            {
                
            }
            return null;
        }
        public Permit FindByPermitNumber(string permitno) =>
            _context.Permits
            .Include("Application.User.Company")
            .Include("Application.ApplicationType")
            .Include("Application.Quarter")
            .Include("Application.Terminal")
            .Include("Application.Product")
            .FirstOrDefault(x => x.PermitNumber.Equals(permitno));

        public Permit FindById(int id) => 
            _context.Permits
            .Include("Application.User.Company")
            .Include("Application.ApplicationType")
            .Include("Application.Quarter")
            .Include("Application.Terminal")
            .Include("Application.Product")
            .FirstOrDefault(x => x.Id == id);

        public Permit Update(Permit permit)
        {
            _context.Permits.Update(permit);
            _context.SaveChanges();
            return permit;
        }
        public Permit Insert(Permit item)
        {
            _context.Permits.Update(item);
            _context.SaveChanges();
            return item;
        }

        public List<Permit> GetCompanyPermits(string userid) =>
            _context.Permits.Where(x => x.UserId == userid).ToList();

        public List<Permit> All() => _context.Permits.Include("Application.User.Company")
            .Include("Application.Quarter")
            .Include("Application.Terminal")
            .Include("Application.Product")
            .ToList();

        private string GeneratePermitNo(string status)
        {
            string no = "DPR/GATEX/"; 
            generate:
            int digits = new Random().Next(10001, 99999);
            if (!string.IsNullOrEmpty(status) && status.Equals("r", StringComparison.OrdinalIgnoreCase))
                no += DateTime.UtcNow.Year.ToString().Substring(2, 2) + "/R{0}";
            else if(!string.IsNullOrEmpty(status) && status.Equals("n", StringComparison.OrdinalIgnoreCase))
                no += DateTime.UtcNow.Year.ToString().Substring(2, 2) + "/N{0}";

            var permitnumber = string.Format(no, digits.ToString("00000"));
            ;

            var exist = FindByPermitNumber(permitnumber);
            if (exist == null)
                return permitnumber;
            else
                goto generate;
        }
        private Permit GeneratePermitExpiry(Permit permit, DateTime date, string quarter)
        {
            int addToYr = 0;
            switch (quarter)
            {
                case "First Quarter":
                    if (date.Month >= 3)
                        addToYr = 1;
                    permit.ExpiryDate = new DateTime(date.Year + addToYr, 3, 31);
                    break;
                case "Second Quarter":
                    if (date.Month >= 6)
                        addToYr = 1;
                    permit.ExpiryDate = new DateTime(date.Year + addToYr, 6, 30);
                    break;
                case "Third Quarter":
                    if (date.Month >= 9)
                        addToYr = 1;
                    permit.ExpiryDate = new DateTime(date.Year, 9, 30);
                    break;
                case "Fourth Quarter":
                    permit.ExpiryDate = new DateTime(date.Year, 12, 31);
                    break;
                default:
                    break;
            }
            return permit;
        }
    }
}