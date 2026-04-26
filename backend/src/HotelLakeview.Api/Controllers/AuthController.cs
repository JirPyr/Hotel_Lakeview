using HotelLakeview.Application.Auth.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelLakeview.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return result.Error.Type switch
            {
                Application.Common.Results.ErrorType.Validation =>
                    BadRequest(new
                    {
                        error = result.Error.Code,
                        message = result.Error.Message
                    }),

                Application.Common.Results.ErrorType.Unauthorized =>
                    Unauthorized(new
                    {
                        error = result.Error.Code,
                        message = result.Error.Message
                    }),

                _ => BadRequest(new
                {
                    error = result.Error.Code,
                    message = result.Error.Message
                })
            };
        }

        return Ok(result.Value);
    }
}