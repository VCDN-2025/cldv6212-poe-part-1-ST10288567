using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using AbcRetailStorage_st10288567.Models;
using static System.Net.Mime.MediaTypeNames;

namespace AbcRetailStorage_st10288567.Controllers
{
    public class ProductsController : Controller
    {
        private readonly TableClient _productTable;
        private readonly BlobContainerClient _blobContainer;

        public ProductsController(TableClient productTable, BlobContainerClient blobContainer)
        {
            _productTable = productTable;
            _blobContainer = blobContainer;
        }

        // List products
        public async Task<IActionResult> Index()
        {
            var items = _productTable.QueryAsync<ProductEntity>(filter: $"PartitionKey eq 'Product'");
            var list = new List<ProductEntity>();
            await foreach (var e in items) list.Add(e);
            return View(list);
        }

        // Details
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var resp = await _productTable.GetEntityAsync<ProductEntity>("Product", id);
            var product = resp.Value;

            string imageUrl = null;
            if (!string.IsNullOrEmpty(product.ImageBlobName))
            {
                var blobClient = _blobContainer.GetBlobClient(product.ImageBlobName);
                imageUrl = blobClient.Uri.ToString(); // public URI
            }

            ViewBag.ImageUrl = imageUrl;
            return View(product);
        }

        // Create form
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductEntity model, IFormFile image)
        {
            model.PartitionKey = "Product";
            model.RowKey = Guid.NewGuid().ToString();

            if (image != null && image.Length > 0)
            {
                var blobName = $"{model.RowKey}_{Path.GetFileName(image.FileName)}";
                var blobClient = _blobContainer.GetBlobClient(blobName);

                using (var stream = image.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = image.ContentType });
                }

                model.ImageBlobName = blobName;
            }

            await _productTable.AddEntityAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // Edit
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var resp = await _productTable.GetEntityAsync<ProductEntity>("Product", id);
            return View(resp.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductEntity model, IFormFile? image)
        {
            if (image != null && image.Length > 0)
            {
                var blobName = $"{model.RowKey}_{Path.GetFileName(image.FileName)}";
                var blobClient = _blobContainer.GetBlobClient(blobName);

                using (var stream = image.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                model.ImageBlobName = blobName;
            }

            await _productTable.UpdateEntityAsync(model, model.ETag, TableUpdateMode.Replace);
            return RedirectToAction(nameof(Index));
        }

        // Delete
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            var resp = await _productTable.GetEntityAsync<ProductEntity>("Product", id);
            return View(resp.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string rowKey)
        {
            var resp = await _productTable.GetEntityAsync<ProductEntity>("Product", rowKey);
            var product = resp.Value;

            // Delete blob if exists
            if (!string.IsNullOrEmpty(product.ImageBlobName))
            {
                var blobClient = _blobContainer.GetBlobClient(product.ImageBlobName);
                await blobClient.DeleteIfExistsAsync();
            }

            await _productTable.DeleteEntityAsync("Product", rowKey);
            return RedirectToAction(nameof(Index));
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
// https://learn.microsoft.com/aspnet/core/mvc/models/file-uploads?view=aspnetcore-8.0