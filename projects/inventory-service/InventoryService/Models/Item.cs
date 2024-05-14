namespace InventoryService.Models;

public class Item
{
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
}

public class ItemDto
{
        public string Name { get; set; }
        public int Quantity { get; set; }
}