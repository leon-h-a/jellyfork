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
        public IActionResult GetMedia([FromForm] string inputDirectory)
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

        [HttpPost("manipulate")]
        public IActionResult Manipulate(
            [FromForm] string inputDirectory,
            [FromForm] string outputDirectory,
            [FromForm] string newName,
            [FromForm] List<String> files,
            [FromForm] bool nest,
            [FromForm] bool copy
        )
        {
            try
            {
                string nestedDir = string.Empty;
                if (nest)
                {
                    nestedDir = Path.Combine(outputDirectory, newName);
                    System.IO.Directory.CreateDirectory(nestedDir);
                }

                foreach (var file in files)
                {
                    string oldFilePath = Path.Combine(inputDirectory, file);
                    _logger.LogInformation(oldFilePath);
                    string extension = Path.GetExtension(file);

                    string newFilePath;
                    if (nest)
                    {
                        newFilePath = Path.Combine(nestedDir, newName + extension);
                    }
                    else
                    {
                        newFilePath = Path.Combine(outputDirectory, newName + extension);
                    }

                    _logger.LogInformation(newFilePath);
                    if (copy)
                    {
                        System.IO.File.Copy(oldFilePath, newFilePath);
                    }
                    else
                    {
                        System.IO.File.Move(oldFilePath, newFilePath);
                    }
                }
                return Ok(new { alert = "Action success"});
            }
            catch (Exception ex)
            { 
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { alert = $"{ex.Message}"});
            }
        }

        [HttpPost("deleteDirectory")]
        public IActionResult DeleteDirectory([FromForm] string path)
        {
            if (Directory.Exists(path))
            {
                System.IO.Directory.Delete(path, true);
                return Ok(new {
                        alert = "Successfully deleted directory",
                        data = path 
                    }
                );
            }
            else
            {
                return BadRequest(new { alert = "Unable to delete directory" });
            }
        }
    }
}
