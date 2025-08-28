using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Storage.Files.Shares;

var builder = WebApplication.CreateBuilder(args);

// Read config
var azureConfig = builder.Configuration.GetSection("AzureStorage");
var connectionString = azureConfig.GetValue<string>("ConnectionString");
var blobContainerName = azureConfig.GetValue<string>("BlobContainer");
var tableCustomers = azureConfig.GetValue<string>("TableCustomers");
var tableProducts = azureConfig.GetValue<string>("TableProducts");
var queueOrders = azureConfig.GetValue<string>("QueueOrders");
var fileShareContracts = azureConfig.GetValue<string>("FileShareContracts");

// Add services
builder.Services.AddControllersWithViews();

// Register clients for DI
builder.Services.AddSingleton(new BlobServiceClient(connectionString));
builder.Services.AddSingleton(sp =>
{
    var client = new BlobServiceClient(connectionString);
    return client.GetBlobContainerClient(blobContainerName);
});
builder.Services.AddSingleton(sp => new TableServiceClient(connectionString));
builder.Services.AddSingleton(sp => new QueueClient(connectionString, queueOrders));
builder.Services.AddSingleton(sp => new ShareClient(connectionString, fileShareContracts));

// Optional: register TableClients for specific tables (created at startup)
builder.Services.AddSingleton(sp => new TableClient(connectionString, tableCustomers));
builder.Services.AddSingleton(sp => new TableClient(connectionString, tableProducts));

// Build app
var app = builder.Build();

// Ensure storage artifacts exist 
using (var scope = app.Services.CreateScope())
{
    var blobContainerClient = scope.ServiceProvider.GetRequiredService<BlobContainerClient>();
    await blobContainerClient.CreateIfNotExistsAsync();

    var tableServiceClient = scope.ServiceProvider.GetRequiredService<TableServiceClient>();
    await tableServiceClient.CreateTableIfNotExistsAsync(tableCustomers);
    await tableServiceClient.CreateTableIfNotExistsAsync(tableProducts);

    var queueClient = scope.ServiceProvider.GetRequiredService<QueueClient>();
    await queueClient.CreateIfNotExistsAsync();

    var shareClient = scope.ServiceProvider.GetRequiredService<ShareClient>();
    await shareClient.CreateIfNotExistsAsync();
}

// MVC pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();




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