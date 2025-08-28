using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;   // <-- for List<T>
using System.Threading.Tasks;       // <-- for async Task
using AbcRetailStorage_st10288567.Models;  // <-- for CustomerEntity

namespace AbcRetailStorage_st10288567.Controllers
{
    public class CustomersController : Controller
    {
        private readonly TableClient _tableClient;

        public CustomersController(TableClient tableClient)
        {
            _tableClient = tableClient;
        }

        // List customers
        public async Task<IActionResult> Index()
        {
            var items = _tableClient.QueryAsync<CustomerEntity>(filter: $"PartitionKey eq 'Customer'");
            var list = new List<CustomerEntity>();
            await foreach (var e in items) list.Add(e);
            return View(list);
        }

        // Details
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();
            var entity = await _tableClient.GetEntityAsync<CustomerEntity>("Customer", id);
            return View(entity.Value);
        }

        // Create form
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerEntity model)
        {
            model.PartitionKey = "Customer";
            model.RowKey = Guid.NewGuid().ToString();
            await _tableClient.AddEntityAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // Edit
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var entity = await _tableClient.GetEntityAsync<CustomerEntity>("Customer", id);
            return View(entity.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerEntity model)
        {
            await _tableClient.UpdateEntityAsync(model, model.ETag, TableUpdateMode.Replace);
            return RedirectToAction(nameof(Index));
        }

        // Delete
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            var entity = await _tableClient.GetEntityAsync<CustomerEntity>("Customer", id);
            return View(entity.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string rowKey)
        {
            await _tableClient.DeleteEntityAsync("Customer", rowKey);
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
// https://learn.microsoft.com/aspnet/c