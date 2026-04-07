using SV22T1020439.DataLayers.SQLServer;
using SV22T1020439.Models;
using System.Collections.Generic;

namespace SV22T1020439.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng xử lý dữ liệu liên quan đến từ điển dữ liệu
    /// </summary>
    public static class DictionaryDataService
    {
        private static readonly ProvinceDAL provinceDB;

        /// <summary>
        /// Constructor
        /// </summary>
        static DictionaryDataService()
        {
            provinceDB = new ProvinceDAL(Configuration.ConnectionString);
        }

        /// <summary>
        /// Lấy danh sách tỉnh thành
        /// </summary>
        public static List<Province> ListOfProvinces()
        {
            return provinceDB.List();
        }
    }
}
