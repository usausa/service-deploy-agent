namespace Service.Deploy.Agent.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    using Service.Deploy.Agent.Settings;

    [ApiController]
    [Route("[controller]/[action]")]
    public class DeployController : ControllerBase
    {
        private ServiceSetting serviceSetting;

        public DeployController(IOptionsSnapshot<ServiceSetting> serviceSetting)
        {
            this.serviceSetting = serviceSetting.Value;
        }

        [HttpPost("{name}")]
        public IActionResult Update(string name, IFormFile archive, [FromQuery] string? token)
        {
            return Ok();
        }
    }
}
