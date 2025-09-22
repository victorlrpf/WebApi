using System.Net.Http.Json;
using System.Text.Json.Serialization;
using BackendExample.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BackendExample.Services;

public class SupabaseService : ISupabaseService
{
    private readonly HttpClient _client;

    public SupabaseService(IHttpClientFactory http)
    {
        _client = http.CreateClient("supabase");
    }

    // Supabase uses PostgREST for DB access. We'll perform simple REST calls to the tables.

    public async Task<UserRecord?> GetUserByEmailAsync(string email)
    {
        // GET /rest/v1/users?email=eq.<email>&select=*
        var resp = await _client.GetAsync($"/rest/v1/users?email=eq.{Uri.EscapeDataString(email)}&select=*");
        if (!resp.IsSuccessStatusCode) return null;
        var text = await resp.Content.ReadAsStringAsync();
        var arr = JArray.Parse(text);
        if (arr.Count == 0) return null;
        return arr[0].ToObject<UserRecord>();
    }

    public async Task<UserRecord?> CreateUserAsync(UserRecord user)
    {
        var body = JsonConvert.SerializeObject(new
        {
            id = user.Id,
            email = user.Email,
            passwordhash = user.PasswordHash,
            fullname = user.FullName,
            createdat = user.CreatedAt
        });
        var resp = await _client.PostAsync("/rest/v1/users", new StringContent(body, System.Text.Encoding.UTF8, "application/json"));
        if (!resp.IsSuccessStatusCode) return null;
        var text = await resp.Content.ReadAsStringAsync();
        var arr = JArray.Parse(text);
        return arr[0].ToObject<UserRecord>();
    }

    public async Task<IEnumerable<StockItem>> GetStockAsync()
    {
        var resp = await _client.GetAsync($"/rest/v1/stock?select=*");
        resp.EnsureSuccessStatusCode();
        var text = await resp.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<IEnumerable<StockItem>>(text)!;
    }

    public async Task<StockItem?> GetStockItemAsync(Guid id)
    {
        var resp = await _client.GetAsync($"/rest/v1/stock?id=eq.{id}&select=*");
        if (!resp.IsSuccessStatusCode) return null;
        var text = await resp.Content.ReadAsStringAsync();
        var arr = JArray.Parse(text);
        if (arr.Count == 0) return null;
        return arr[0].ToObject<StockItem>();
    }

    public async Task<StockItem?> CreateStockItemAsync(StockItem item)
    {
        var body = JsonConvert.SerializeObject(item);
        var resp = await _client.PostAsync("/rest/v1/stock", new StringContent(body, System.Text.Encoding.UTF8, "application/json"));
        if (!resp.IsSuccessStatusCode) return null;
        var text = await resp.Content.ReadAsStringAsync();
        var arr = JArray.Parse(text);
        return arr[0].ToObject<StockItem>();
    }

    public async Task<bool> UpdateStockQuantityAsync(Guid id, int delta)
    {
        // we can fetch, update and PUT (patch) the record
        var item = await GetStockItemAsync(id);
        if (item == null) return false;
        item.Quantity += delta;
        item.UpdatedAt = DateTime.UtcNow;
        var body = JsonConvert.SerializeObject(new { quantity = item.Quantity, updatedat = item.UpdatedAt });
        var resp = await _client.PatchAsync($"/rest/v1/stock?id=eq.{id}", new StringContent(body, System.Text.Encoding.UTF8, "application/json"));
        return resp.IsSuccessStatusCode;
    }
}
