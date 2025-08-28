using Azure;
using Azure.Data.Tables;
using System;

namespace AbcRetailStorage_st10288567.Models
{
    public class ProductEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = "Product"; // e.g. "Product"
        public string RowKey { get; set; } = Guid.NewGuid().ToString(); // default GUID
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        // Product properties
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public double Price { get; set; } = 0;
        public int Stock { get; set; } = 0;
        public string ImageBlobName { get; set; } = ""; // blob filename
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