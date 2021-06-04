using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using GOTEX.Core.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GOTEX.Core.DAL
{
    public class MessageRepository : IRepository<Message>
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public MessageRepository(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public Message Insert(Message item)
        {
            try
            {
                _context.Messages.Add(item);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                
            }
            return item;
        }
        public Message Update(Message item)
        {
            try
            {
                _context.Messages.Update(item);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                
            }
            return item;
        }
        public Message Remove(int id)
        {
            throw new System.NotImplementedException();
        }

        public Message FindById(int id) => _context.Messages.FirstOrDefault(x => x.Id == id);
        public List<Message> GetAll() => _context.Messages.ToList();
        public List<Message> GetListByUserId(string id) => _context.Messages.Include("User").Where(x => x.UserId == id).ToList();
        public bool SendApplicationMessage(Application application, string webrootpath, List<MailTemplate> mailTemplates, Dictionary<string, string> mailSettings)
        {
            try
            {
                string subject = (application.ApplicationTypeId == 1 ? "Application Initiated: " 
                    : "Permit Renewal Initiated: ") + application.Reference;

                var prevmessage = _context.Messages.FirstOrDefault(x => x.Subject.Equals(subject));
                if (prevmessage == null)
                {
                    var msg = new Message
                    {
                        User = application.User,
                        Date = DateTime.UtcNow.AddHours(1),
                        Subject = subject,
                        Content = "Loading...",
                    };
                    msg = Insert(msg);
                    
                    var body = Utils.ReadTextFile(webrootpath, "GeneralFormat.txt");

                    string mailtype = application.ApplicationType.FullName;
                    
                    string appInfo = string.Format(mailTemplates.FirstOrDefault(x => x.Type.ToLower() == "new" && x.Category.Equals("company", StringComparison.OrdinalIgnoreCase))?.Content ?? "",
                        application.Reference, application.ApplicationType.Name, "Export Permit Approval", application.Fee + application.ServiceCharge, application.Year, mailtype);
                    
                    var msgBody = string.Format(body, subject, appInfo, msg.Id, mailtype, "https://cogatex.dpr.gov.ng");
                    msg.Content = msgBody;
                    Update(msg);
                    
                    Utils.SendMail(mailSettings, application.User.Email, subject, msgBody);

                }
                return true;
            }
            catch (Exception ex)
            {
                
            }
            return false;
        }

        public string SendApplicationSubmittedMail(Application application, Dictionary<string, string> mailsettings, string webrootpath)
        {
            string desc = "";
            try
            {
                #region Send Application Submitted Mail

                string subject = string.Empty;
                string tk = string.Empty;
                string tbl = string.Empty;
                if (application.Submitted)
                {
                    #region Resubmission
                    subject = "Application Re-submitted - Ref. No.: " + application.Reference;
                    tk = string.Format("Your application has been re-submitted on DPR COGATEX portal. The process for your application with reference number: {0} will continue immediately.<br />", application.Reference);
                    desc = "Resubmit";
                    #endregion
                }
                else
                {
                    #region Fresh Submission
                    desc = "New";
                    subject = ("Application Submitted - Ref. No.: ") + application.Reference;
                    tk = string.Format("Thank you for submitting your application on the COGATEX portal. Your application with reference number: {0} will be reviewed <br />The table below shows the breakdown of the funds received.", application.Reference);
                    tbl = "<table><tr><td><b>Company Name</b></td><td>{0}</td></tr>" +
                        "<tr><td><b>Quarter:</b></td><td>{7}</td></tr>" +
                        "<tr><td><b>Reference Number:</b></td><td>{1}</td></tr>" +
                        "<tr><td><b>Statutory Licence Fee:</b></td><td>{2}</td></tr>" +
                        "<tr><td><b>Service Charge:</b></td><td>{3}</td></tr>" +
                        "<tr><td><b>Total Amount Due:</b></td><td>{4}</td></tr>" +
                        "<tr><td><b>Application Period:</b></td><td>{5}</td></tr>" +
                        "<tr><td><b>Date Submitted</b></td><td>{6}</td></tr></table>";
                    #endregion
                }
                var message = new Message
                {
                    Content = "Loading...",
                    Date = DateTime.UtcNow.AddHours(1),
                    Subject = subject,
                    User = application.User,
                };

                _context.Messages.Add(message);
                _context.SaveChanges();

                var sn = message.Id;
                var body = Utils.ReadTextFile(webrootpath, "GeneralFormat.txt");
                var apDetails = "";
                tbl = string.IsNullOrEmpty(tbl) ? "" : string.Format(tbl, application.User.Company.Name, application.Reference,
                    application.Fee, application.ServiceCharge, application.Fee + application.ServiceCharge, application.Year,
                    DateTime.UtcNow.AddHours(1).ToString(), $"{application.Quarter.Name} for {application.Quantity.ToString("N2")} " +
                                                            $"Barrels of {application.Product.Name}", "https://cogatex.dpr..gov.ng");


                var src = "<table class='table table-bordered table-striped'>" +
                     "<tr><td><b>Application Category:</b></td><td>" + application.ApplicationType.Name + "</td></tr>" +
                     "<tr><td><b>Application Quarter</b></td><td>" + application.Quarter.Name + "</td></tr></table>";

                apDetails = tk + tbl + src;

                var msgbody = string.Format(body, subject, apDetails, sn, DateTime.Now.Year);

                message.Content = msgbody;
                _context.Messages.Update(message);
                _context.SaveChanges();

                Utils.SendMail(mailsettings,application.User.Email, message.Subject, msgbody);
                #endregion
            }
            catch (Exception ex)
            {
                
            }
            return desc;
        }
    }
}