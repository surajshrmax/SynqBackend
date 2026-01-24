namespace Synq.Application.DTOs.Auth;

public record AuthResponse(Guid userId, string AccessToken, string RefreshToken);
