using System.Collections.Generic;
using System.Linq;

namespace SrpTask
{
    public class ItemsHelper
    {
        public int CalculateStats(List<Item> inventory)
        {
            return inventory.Sum(x => x.Armour);
        }

        public bool CheckIfItemExistsInInventory(Item item, List<Item> inventory)
        {
            return inventory.Any(x => x.Id == item.Id);
        }

        public int CalculateInventoryWeight(List<Item> inventory)
        {
            return inventory.Sum(x => x.Weight);
        }   
    }
}