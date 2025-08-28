using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbcRetailStorage_st10288567.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ShareClient _shareClient;

        public ContractsController(ShareClient shareClient)
        {
            _shareClient = shareClient;
        }

        // List files in contracts root
        public IActionResult Index()
        {
            var dir = _shareClient.GetRootDirectoryClient();
            var items = dir.GetFilesAndDirectories();

            var files = new List<ShareFileItem>();
            foreach (var item in items) // fixed await foreach
            {
                if (!item.IsDirectory)
                    files.Add(item);
            }

            return View(files);
        }

        // Upload form
        public IActionResult Upload() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile contractFile)
        {
            if (contractFile == null || contractFile.Length == 0)
                return RedirectToAction("Index");

            var root = _shareClient.GetRootDirectoryClient();
            var fileClient = root.GetFileClient(contractFile.FileName);

            await fileClient.CreateAsync(contractFile.Length);

            using (var stream = contractFile.OpenReadStream())
            {
                await fileClient.UploadRangeAsync(new HttpRange(0, contractFile.Length), stream);
            }

            return RedirectToAction(nameof(Index));
        }

        // Download
        public async Task<IActionResult> Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return RedirectToAction("Index");

            var root = _shareClient.GetRootDirectoryClient();
            var fileClient = root.GetFileClient(fileName);

            var download = await fileClient.DownloadAsync();
            return File(download.Value.Content, "application/octet-stream", fileName);
        }
    }
}




// References


// ASP.NET Core MVC
// Microsoft Docs: Introduction to ASP.NET Core MVC


// Azure Table Storage
// Microsoft Docs: Azure Table storage overview
// https://learn.microsoft.com/azure/storage/tables/table-storage-overview



// Azure Blob Storage
// Microsoft Docs: Azure Blob storage overview
// https://learn.microsoft.com/azure/storage/blobs/storage-blobs-introduction
// Microsoft Docs: Quickstart: Manage blobs with .NET


// File Uploads & Images in ASP.NET Core
// Microsoft Docs: Upload files in ASP.NET Core
// https://learn.microsoft.com/aspnet/c