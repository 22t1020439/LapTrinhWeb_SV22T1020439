using System.Collections.Generic;

namespace SV22T1020439.DataLayers.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        List<T> List(int page = 1, int pageSize = 0, string searchValue = "");
        int Count(string searchValue = "");
        T? Get(int id);
        int Add(T data);
        bool Update(T data);
        bool Delete(int id);
        bool InUsed(int id);
    }
}

