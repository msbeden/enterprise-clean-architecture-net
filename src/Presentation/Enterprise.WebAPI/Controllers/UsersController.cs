using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Users.Commands.CreateUser;
using Enterprise.Application.Features.Users.DTOs;
using Enterprise.Application.Features.Users.Queries.GetUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enterprise.WebAPI.Controllers;

/// <summary>
/// API Controller for managing users.
/// Requires authentication (session/token).
/// </summary>
[Authorize]
public class UsersController : ApiControllerBase
{
    /// <summary>
    /// Creates a new user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all users.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Result<List<UserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetUsersQuery(), cancellationToken);
        return Ok(result);
    }
}
