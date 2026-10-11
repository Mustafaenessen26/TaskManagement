using TaskManagement.Application.Common.Models;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    JwtToken Generate(User user);
}