using Enterprise.Application.Common.Models;
using Enterprise.Application.Features.Categories.Commands.CreateCategory;
using Enterprise.Application.Features.Categories.DTOs;
using Enterprise.Application.Features.Categories.Queries.GetCategories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enterprise.WebAPI.Controllers;

/// <summary>
/// API Controller for managing categories.
/// Publicly accessible without authentication.
/// </summary>
[AllowAnonymous]
public class CategoriesController : ApiControllerBase
{
    /// <summary>
    /// Creates a new category.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all categories.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Result<List<CategoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetCategoriesQuery(), cancellationToken);
        return Ok(result);
    }
}
