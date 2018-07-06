namespace SrpTask
{
    public class Item
    {
        /// <summary>
        /// Items unique Id;
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Items name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// How much the item heals by.
        /// </summary>
        public int Heal { get; set; }

        /// <summary>
        /// How much armour the player gets when it is equipped.
        /// </summary>
        public int Armour { get; set; }

        /// <summary>
        /// How much this item weighs in kilograms.
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// A unique item can only be picked up once.
        /// </summary>
        public bool Unique { get; set; }

        /// <summary>
        /// Rare items are posh and shiny
        /// </summary>
        public readonly bool Rare;

        public Item(int id, string name, int heal, int armour, int weight, bool unique, bool rare)
        {
            Rare = rare;
            Name = name;
            Heal = heal;
            Armour = armour;
            Weight = weight;
            Unique = unique;
            Id = id;
        }
    }
}