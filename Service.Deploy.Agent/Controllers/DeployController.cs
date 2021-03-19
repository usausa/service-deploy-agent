namespace Service.Deploy.Agent.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]/[action]")]
    public class DeployController : ControllerBase
    {
        [HttpPost]
        public IActionResult Update()
        {
            return Ok();
        }
    }
}
