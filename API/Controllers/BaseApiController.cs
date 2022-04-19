using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices
            .GetService<IMediator>();
        
        protected ActionResult HandleResult<T>(Result<T> result)
        {
            //Data not found result
            if (result == null) return NotFound();
            //Good request with data
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            //Data not found result
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            //Obviously anything else
            return BadRequest(result.Error);
        }
    }
}