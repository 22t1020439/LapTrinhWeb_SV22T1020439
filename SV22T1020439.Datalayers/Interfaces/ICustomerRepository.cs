using SV22T1020439.Models;

namespace SV22T1020439.DataLayers.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        bool ValidateEmail(string email, int id = 0);
        bool ChangePassword(int id, string password);
    }
}

