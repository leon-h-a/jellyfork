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

        [HttpPost("setdirectories")]
        public IActionResult SetDirectries(
                [FromForm] string inputDirectory,
                [FromForm] string outputDirectory
            )
        {
            if (!Directory.Exists(inputDirectory))
            {
                return BadRequest("Downloads directory does not exist");
            }
            else if (!Directory.Exists(outputDirectory))
            {
                return BadRequest("Media directory does not exist");
            }
            else
            {
                return Ok("Success!");
            }
        }

        [HttpPost("getmedia")]
        public IActionResult GetMedia([FromForm] string inputDirectory)
        {
            var directoryTree = GetDirectoryTree(inputDirectory);
            string json = JsonSerializer.Serialize(directoryTree, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            return Ok(new { data = json });
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
                if (copy) {
                    return Ok("Copy successful!");
                }
                else {
                    return Ok("Move successful!");
                }
            }
            catch (Exception ex)
            { 
                _logger.LogError($"{ex.Message}");
                return BadRequest($"{ex.Message}");
            }
        }

        [HttpPost("deleteDirectory")]
        public IActionResult DeleteDirectory([FromForm] string path)
        {
            if (Directory.Exists(path))
            {
                System.IO.Directory.Delete(path, true);
                return Ok(new {
                        msg = "Directory successfully deleted",
                        data = path 
                    }
                );
            }
            else
            {
                return BadRequest("Unable to delete directory");
            }
        }
    }
}
