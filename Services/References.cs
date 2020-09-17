using Globaltec.Models;

namespace Globaltec.Services
{
    public interface ITokenService
    {
        public string generateToken(IUser user);
    }
}