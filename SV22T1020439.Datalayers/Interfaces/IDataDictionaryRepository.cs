using System.Collections.Generic;

namespace SV22T1020439.DataLayers.Interfaces
{
    public interface IDataDictionaryRepository<T> where T : class
    {
        List<T> List();
    }
}

