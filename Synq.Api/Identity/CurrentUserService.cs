using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Synq.Application.Common.Interfaces;

namespace Synq.Infrastructure.Identity;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid UserId
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity!.IsAuthenticated)
            {
                throw new UnauthorizedAccessException();
            }

            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            return Guid.Parse(id);
        }
    }

    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}