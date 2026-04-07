using Dapper;
using SV22T1020439.Models;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace SV22T1020439.DataLayers.SQLServer
{
    public class ProvinceDAL
    {
        private string _connectionString;

        public ProvinceDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Province> List()
        {
            List<Province> data = new List<Province>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "select * from Provinces";
                data = connection.Query<Province>(sql).ToList();
            }
            return data;
        }
    }
}
