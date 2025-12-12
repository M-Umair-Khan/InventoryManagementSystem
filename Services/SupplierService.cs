using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ApplicationDbContext _context;

        public SupplierService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Supplier>> GetAllSuppliersAsync()
        {
            return await _context.Suppliers
                .Where(s => s.IsActive)
                .OrderBy(s => s.SupplierName)
                .ToListAsync();
        }

        public async Task<Supplier> GetSupplierByIdAsync(int id)
        {
            return await _context.Suppliers
                .Include(s => s.Products)
                .Include(s => s.PurchaseOrders)
                .FirstOrDefaultAsync(s => s.SupplierID == id);
        }

        public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
        {
            supplier.CreatedDate = DateTime.Now;
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return supplier;
        }

        public async Task<Supplier> UpdateSupplierAsync(Supplier supplier)
        {
            supplier.UpdatedDate = DateTime.Now;
            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();
            return supplier;
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return false;

            supplier.IsActive = false;
            supplier.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}