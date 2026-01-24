using Synq.Domain.Entities;

namespace Synq.Application.Common.Interfaces.Repositories;

public interface IUserRepository
{
    void AddAsync(User user);
}