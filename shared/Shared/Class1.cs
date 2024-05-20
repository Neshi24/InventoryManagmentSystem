namespace Shared;

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

public class Order
{
    public int Id { get; set; }
    public string Address { get; set; }
    public DateTime OrderDate { get; set; }
    public List<int> ItemsIds { get; set; }
}

public class OrderDto
{
    public string Address { get; set; }
    public List<int> ItemsIds { get; set; }
}

public class Message
{
    public int OrderId { get; set; }
    public List<int> ItemsIds { get; set; }
}