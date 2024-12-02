using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

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

        [HttpPost("getmedia")]
        public IActionResult Run([FromForm] string inputDirectory)
        {
            _logger.LogInformation($"input: {inputDirectory}");

            if (Directory.Exists(inputDirectory))
            {
                var directoryTree = GetDirectoryTree(inputDirectory);
                string json = JsonSerializer.Serialize(directoryTree, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                _logger.LogInformation(json);
                return Ok(new { alert = "Configuration saved successfully!", data = json });
            }
            else
            {
                _logger.LogInformation("Downloads dir fail");
                return BadRequest(new { alert = "Downloads directory does not exist" });
            }
        }

        private static DirectoryNode GetDirectoryTree(string path)
        {
            var directoryInfo = new DirectoryInfo(path);

            if (directoryInfo.Attributes.HasFlag(FileAttributes.Hidden))
            {
                return null;
            }

            var directoryNode = new DirectoryNode
            {
                Name = directoryInfo.Name,
                FullPath = directoryInfo.FullName,
                Files = new List<FileNode>(),
                Subdirectories = new List<DirectoryNode>()
            };

            foreach (var file in directoryInfo.GetFiles())
            {
                if (!file.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    directoryNode.Files.Add(new FileNode
                    {
                        Name = file.Name,
                        FullPath = file.FullName
                    });
                }
            }

            foreach (var subdirectory in directoryInfo.GetDirectories())
            {
                directoryNode.Subdirectories.Add(GetDirectoryTree(subdirectory.FullName));
            }

            return directoryNode;
        }

        private class DirectoryNode
        {
            public string Name { get; set; }
            public string FullPath { get; set; }
            public List<FileNode> Files { get; set; }
            public List<DirectoryNode> Subdirectories { get; set; }
        }

        private class FileNode
        {
            public string Name { get; set; }
            public string FullPath { get; set; }
        }
    }
}
