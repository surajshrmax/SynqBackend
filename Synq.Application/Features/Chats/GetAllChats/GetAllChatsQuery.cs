using MediatR;
using Synq.Application.DTOs;

namespace Synq.Application.Features.Chats.GetAllChats;

public record GetAllChatsQuery : IRequest<IEnumerable<ChatDto>>;