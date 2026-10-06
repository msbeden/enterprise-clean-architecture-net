using Enterprise.Application.Common.Interfaces;
using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Users.DTOs;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Application.Features.Users.Queries.GetUsers;

/// <summary>
/// Handler for GetUsersQuery.
/// </summary>
public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<List<UserDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .AsNoTracking()
            .OrderBy(u => u.Email)
            .ProjectToType<UserDto>()
            .ToListAsync(cancellationToken);

        return Result<List<UserDto>>.Success(users);
    }
}
