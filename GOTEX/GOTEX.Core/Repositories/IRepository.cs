using System.Collections.Generic;
using GOTEX.Core.BusinessObjects;

namespace GOTEX.Core.Repositories
{
    public interface IRepository<T> where T : class
    {
        T Insert(T item);
        T Update(T item);
        void Remove(int id);
        T FindById(int id);
        List<T> GetAll();
        List<T> GetListByUserId(string id);
        bool SendApplicationMessage(Application application, string webrootpath, List<MailTemplate> mailTemplates, Dictionary<string, string> mailSettings);
        string SendApplicationSubmittedMail(Application application, Dictionary<string, string> mailsettings, string webrootpath);
        bool DeleteRange(List<T> items);
    }
}