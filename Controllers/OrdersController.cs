using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;      // <-- for List<T>
using System.Text.Json;
using System.Threading.Tasks;

namespace AbcRetailStorage_st10288567.Controllers
{
    public class OrdersController : Controller
    {
        private readonly QueueClient _queueClient;

        public OrdersController(QueueClient queueClient)
        {
            _queueClient = queueClient;
        }

        // Simple UI to push a message to queue describing an operation
        public IActionResult CreateOperation() => View(); // small form in Razor

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOperation(string operationType, string targetName)
        {
            if (string.IsNullOrEmpty(operationType) || string.IsNullOrEmpty(targetName))
                return RedirectToAction("Index");

            // Example payload
            var payload = new
            {
                Operation = operationType, // e.g. "ProcessOrder", "UpdateInventory", "UploadImage"
                Target = targetName,
                Timestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(payload);

            // QueueClient expects a string; base64 is optional but safe
            await _queueClient.SendMessageAsync(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json)));

            return RedirectToAction(nameof(Index));
        }

        // List messages (peek) - useful for debug
        public async Task<IActionResult> Index()
        {
            var peek = await _queueClient.PeekMessagesAsync(maxMessages: 10);
            var list = new List<string>();

            foreach (var msg in peek.Value)
            {
                // Decode from Base64
                var text = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(msg.MessageText));
                list.Add(text);
            }

            return View(list);
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