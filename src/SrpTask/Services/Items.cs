using System.Collections.Generic;
using System.Linq;

namespace SrpTask
{
    public class Items
    {
        public const int MaximumCarryingCapacity = 1000;

        private readonly IGameEngine _gameEngine;

        private ItemsHelper ItemHelper;

        public int Health { get; set; }

        public int Armour { get; private set; }

        public int MaxHealth { get; set; }

        public List<Item> Inventory;

        /// <summary>
        /// How much the player can carry in kilograms
        /// </summary>
        public int CarryingCapacity { get; private set; }

        public Items(IGameEngine gameEngine)
        {
            _gameEngine = gameEngine;
            ItemHelper = new ItemsHelper();
            Inventory = new List<Item>();
            CarryingCapacity = MaximumCarryingCapacity;
        }

        public void UseItem(Item item)
        {
            if (item.Name == "Stink Bomb")
            {
                var enemies = _gameEngine.GetEnemiesNear(this);

                foreach (var enemy in enemies)
                {
                    enemy.TakeDamage(100);
                }
            }
        }

        public bool PickUpItem(Item item)
        {
            var weight = ItemHelper.CalculateInventoryWeight(Inventory);
            if (weight + item.Weight > CarryingCapacity)
                return false;

            if (item.Unique && ItemHelper.CheckIfItemExistsInInventory(item, Inventory))
                return false;

            // Don't pick up items that give health, just consume them.
            if (item.Heal > 0)
            {
                Health += item.Heal;

                if (Health > MaxHealth)
                    Health = MaxHealth;

                if (item.Heal > 500)
                {
                    _gameEngine.PlaySpecialEffect("green_swirly");
                }

                return true;
            }

            if (item.Rare && item.Unique)
                _gameEngine.PlaySpecialEffect("blue_swirly");

            if (item.Rare)
                _gameEngine.PlaySpecialEffect("cool_swirly_particles");

            Inventory.Add(item);

            Armour = ItemHelper.CalculateStats(Inventory);

            return true;
        }
    }
}