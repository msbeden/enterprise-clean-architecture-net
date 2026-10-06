using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Users.DTOs;
using MediatR;

namespace Enterprise.Application.Features.Users.Queries.GetUsers;

/// <summary>
/// Query to retrieve all users.
/// </summary>
public record GetUsersQuery() : IRequest<Result<List<UserDto>>>;
