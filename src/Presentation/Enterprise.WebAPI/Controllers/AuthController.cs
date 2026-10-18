using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Auth.Commands.Login;
using Enterprise.Application.Features.Auth.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enterprise.WebAPI.Controllers;

/// <summary>
/// API Controller for authentication and token management.
/// </summary>
[AllowAnonymous]
public class AuthController : ApiControllerBase
{
    /// <summary>
    /// Authenticates a user and generates a JWT Bearer token.
    /// </summary>
    /// <param name="command">User login details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Login response containing JWT token and user info.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(Result<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<LoginResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
