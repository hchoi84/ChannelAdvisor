using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChannelAdvisor.Models
{
  public class LocalInventoryRepo : IInventory
  {
    private List<Inventory> _inventories = new List<Inventory>();

    public async Task<Inventory> AddInventoryAsync(JArray inventory)
    {
      Inventory inv = new Inventory()
      {
        Id = _inventories.Any() ? _inventories.Max(i => i.Id) + 1 : 1,
        ProductId = Convert.ToInt32(inventory[0]["ProductId"]),
        QtyWH = GetValue(inventory, 0),
        QtyFBA = GetValue(inventory, -4),
        Created = DateTime.Now,
      };
      _inventories.Add(inv);

      return inv;
    }

    public int GetValue(JArray inventory, int dcId)
    {
      var obj = inventory.FirstOrDefault(a => Convert.ToInt32(a["DistributionCenterID"]) == dcId);
      if (obj != null)
      {
        return Convert.ToInt32(obj["AvailableQuantity"]);
      }

      return 0;
    }
  }
}