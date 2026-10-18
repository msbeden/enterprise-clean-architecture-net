using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Enterprise.WebAPI.Controllers;

/// <summary>
/// Abstract base API controller providing shared MediatR ISender instance.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _sender;

    protected ISender Sender => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
