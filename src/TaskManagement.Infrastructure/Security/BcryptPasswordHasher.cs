using TaskManagement.Application.Common.Constants;
using TaskManagement.Application.Common.Interfaces;

namespace TaskManagement.Infrastructure.Security;

public class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, SecurityConstants.BcryptWorkFactor);

    public bool Verify(string password, string passwordHash)
        => BCrypt.Net.BCrypt.Verify(password, passwordHash);
}