using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<ISalesOrderService, SalesOrderService>();
builder.Services.AddScoped<IReportService, ReportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed initial data (optional - for development)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();

    // Seed initial data if tables are empty
    if (!dbContext.Categories.Any())
    {
        dbContext.Categories.AddRange(
            new Category { CategoryName = "Electronics", Description = "Electronic items" },
            new Category { CategoryName = "Office Supplies", Description = "Office stationery and supplies" },
            new Category { CategoryName = "Furniture", Description = "Office furniture" }
        );
        await dbContext.SaveChangesAsync();
    }

    if (!dbContext.Warehouses.Any())
    {
        dbContext.Warehouses.AddRange(
            new Warehouse
            {
                WarehouseCode = "WH01",
                WarehouseName = "Main Warehouse",
                Location = "123 Main St, City",
                Capacity = 10000
            }
        );
        await dbContext.SaveChangesAsync();
    }
}

app.Run();