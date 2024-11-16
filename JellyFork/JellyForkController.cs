using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.IO;
using System.Collections;
using MediaBrowser.Common.Plugins;


namespace Jellyfin.JellyForkPlugin.JellyFork
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

        [HttpPost("getMedia")]
        public IActionResult Run([FromForm] string inputDirectory)
        {
            _logger.LogInformation(inputDirectory);
            if (!Directory.Exists(inputDirectory)) {
                _logger.LogInformation("Downloads dir fail");
                return BadRequest(new { alert = "Downloads directory does not exist" });
            } else {
                _logger.LogInformation("OK");
                return Ok(new { alert = "Configuration saved successfully!" });
            }

        }

        /* [HttpGet("getDefault")] */
        /* public IActionResult GetConfiguration() */
        /* { */
        /*     if (_plugin != null) */
        /*     { */
        /*         var config = _plugin.Configuration; */
        /*         return Ok(new */
        /*         { */
        /*             InputDirectory = config.InputDirectory, */
        /*             OutputDirectory = config.OutputDirectory */
        /*         }); */
        /*     } */

        /*     return NotFound(new { alert = "Plugin configuration not found" }); */
        /* } */
    }
}
