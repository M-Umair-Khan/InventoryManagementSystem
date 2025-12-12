using InventoryManagementSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Services
{
    public interface ISupplierService
    {
        Task<List<Supplier>> GetAllSuppliersAsync();
        Task<Supplier> GetSupplierByIdAsync(int id);
        Task<Supplier> CreateSupplierAsync(Supplier supplier);
        Task<Supplier> UpdateSupplierAsync(Supplier supplier);
        Task<bool> DeleteSupplierAsync(int id);
    }
}