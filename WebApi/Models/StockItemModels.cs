namespace BackendExample.Models;

public class StockItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public int Quantity { get; set; }
    public DateTime UpdatedAt { get; set; }
}