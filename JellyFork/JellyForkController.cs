using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
            if (Directory.Exists(inputDirectory))
            {
                var directoryTree = GetDirectoryTree(inputDirectory);
                string json = JsonSerializer.Serialize(directoryTree, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

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
            public required string Name { get; set; }
            public required string FullPath { get; set; }
            public required List<FileNode> Files { get; set; }
            public required List<DirectoryNode> Subdirectories { get; set; }
        }

        private class FileNode
        {
            public required string Name { get; set; }
            public required string FullPath { get; set; }
        }
        [HttpPost("rename")]
        public IActionResult Run(
            [FromForm] string inputDirectory,
            [FromForm] string outputDirectory,
            [FromForm] string newName,
            [FromForm] List<String> files
        )
        {
            _logger.LogDebug(inputDirectory);
            _logger.LogDebug(outputDirectory);
            _logger.LogDebug(newName);
            _logger.LogDebug(string.Join(", ", files));

            try
            {
                foreach (var file in files)
                {
                    string oldFilePath = Path.Combine(inputDirectory, file);
                    _logger.LogInformation(oldFilePath);
                    string extension = Path.GetExtension(file);
                    string newFilePath = Path.Combine(outputDirectory, newName + extension);
                    _logger.LogInformation(newFilePath);

                    System.IO.File.Move(oldFilePath, newFilePath);
                }
                return Ok(new { alert = "Action success"});
            }
            catch (Exception ex)
            { 
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { alert = $"{ex.Message}"});
            }
        }
    }
}
