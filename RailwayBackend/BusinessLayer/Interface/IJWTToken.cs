using System;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface;

public interface IJwtToken
{
    string GenerateToken(User user);
    string ValidateToken(string token);
}
