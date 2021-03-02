using System.Collections.Generic;

namespace GOTEX.Core.Repositories
{
    public interface IApplicationTypeDocs<T> where T : class
    {
        bool InsertDocs(T item);
        List<T> GetAll();
        bool Delete(T item);
    }
}