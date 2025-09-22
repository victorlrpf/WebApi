using BackendExample.Models;

namespace BackendExample.Services;

public interface ISupabaseService
{
    Task<UserRecord?> GetUserByEmailAsync(string email);
    Task<UserRecord?> CreateUserAsync(UserRecord user);
    Task<IEnumerable<StockItem>> GetStockAsync();
    Task<StockItem?> GetStockItemAsync(Guid id);
    Task<StockItem?> CreateStockItemAsync(StockItem item);
    Task<bool> UpdateStockQuantityAsync(Guid id, int delta);
}