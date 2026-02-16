using System.Linq.Expressions;
using Synq.Application.DTOs;
using Synq.Domain.Entities;

namespace Synq.Application.Mappers;

public static class UserMapper
{
  public static UserDto? ToDto(this User user)
  {
    if (user == null) return null;
    return new UserDto
    {
      Id = user.Id,
      Email = user.Email,
      Username = user.Username,
      UserProfile = new UserProfileDto
      {
        Name = user.UserProfile.Name,
        Bio = user.UserProfile.Bio,
        ImageUrl = user.UserProfile.ImageUrl,
        LastSeenAt = user.UserProfile.LastSeenAt
      }
    };
  }

  public static Expression<Func<User, UserDto>> ToDtoExpr = user => new UserDto
  {
    Id = user.Id,
    Email = user.Email,
    Username = user.Username,
    UserProfile = new UserProfileDto
    {
      Name = user.UserProfile.Name,
      Bio = user.UserProfile.Bio,
      ImageUrl = user.UserProfile.ImageUrl,
      LastSeenAt = user.UserProfile.LastSeenAt
    }
  };
}
