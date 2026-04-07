using SV22T1020439.Models;

namespace SV22T1020439.DataLayers.Interfaces
{
    public interface IUserAccountRepository
    {
        Employee? Authorize(string email, string password);
        bool ChangePassword(string email, string password);
    }
}

