using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_opentelemetry2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegistryController : ControllerBase
    {

        private readonly ILogger<RegistryController> _logger;
        public RegistryController(ILogger<RegistryController> logger)
        {
            _logger = logger;
        }

        [HttpGet()]
        public IActionResult GetRegistry([FromQuery] string type)
        {
            // Simulate some processing logic
            if (string.IsNullOrEmpty(type))
            {
                _logger.LogWarning("Type parameter is null or empty.");
                return BadRequest("Type cannot be null or empty.");
            }

            // Here you would typically fetch data based on the type parameter
            // For demonstration, we will just return a simple message
            var result = $"Registry data for type: {type}";

            switch (type.ToLower())
            {
                case "info":
                    _logger.LogInformation("Fetching registry data for Type 1");
                    break;
                case "error":
                    _logger.LogError("Fetching registry data for Type 2");
                    break;
                case "warning":
                    _logger.LogWarning("Fetching registry data for Type 3");
                    break;
                case "debug":
                    _logger.LogDebug("Fetching registry data for Type 4");
                    break;
                case "trace":
                    _logger.LogTrace("Fetching registry data for Type 5");
                    break;
                case "critical":
                    _logger.LogCritical("Fetching registry data for Type 6");
                    break;
                default:
                    _logger.LogWarning($"Unknown type requested: {type}");
                    return NotFound($"No registry data found for type: {type}");
            }
            
            return Ok(result);
        }
    }
}
