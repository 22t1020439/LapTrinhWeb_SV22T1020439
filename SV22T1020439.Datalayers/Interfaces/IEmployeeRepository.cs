using SV22T1020439.Models;

namespace SV22T1020439.DataLayers.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        bool ValidateEmail(string email, int id = 0);
        bool ChangePassword(int id, string password);
        bool ChangeRole(int id, string roleNames);
    }
}

