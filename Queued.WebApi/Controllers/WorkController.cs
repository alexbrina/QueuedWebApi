using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Queued.Application;
using System.Threading.Tasks;

namespace Queued.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post(WorkRequest request,
            [FromServices] IWorkUseCase useCase)
        {
            await useCase.Add(request);
            return Accepted();
        }

        [HttpPost("Requeue")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Requeue(
            [FromServices] IWorkUseCase useCase)
        {
            await useCase.Requeue();
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Health() => Ok();
    }
}
