using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.IO;
using System.Collections;


namespace Jellyfin.Plugin.JellyFork
{
    [ApiController]
    [Route("plugins/jellyfork")]
    public class JellyForkController : ControllerBase
    {
        private readonly ILogger<JellyForkController> _logger;

        public JellyForkController(ILogger<JellyForkController> logger)
        {
            _logger = logger;
        }

        [HttpPost("run")]
        public IActionResult Run([FromForm] string inputDirectory, [FromForm] string outputDirectory)
        {
            _logger.LogDebug($"input: {inputDirectory}");
            _logger.LogDebug($"output: {outputDirectory}");

            if (!Directory.Exists(inputDirectory)) {
                return BadRequest(new { alert = "Downloads directory does not exist" });
            }

            if (!Directory.Exists(outputDirectory)) {
                return BadRequest(new { alert = "Movies directory does not exist" });
            }

            return Ok(new { alert = "Configuration saved successfully!" });
        }
    }
}
