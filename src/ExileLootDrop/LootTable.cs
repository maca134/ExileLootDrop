using System;
using System.Collections.Generic;

namespace ExileLootDrop
{
    public class LootTable
    {
        /// <summary>
        /// Loot table name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// List of loot items
        /// </summary>
        public LootItem[] LootItems { get; }

        private readonly Random _rnd = new Random();

        /// <summary>
        /// LootTable constuctor
        /// </summary>
        /// <param name="name">Table name</param>
        /// <param name="lootList">Item list</param>
        public LootTable(string name, List<LootItem> lootList)
        {
            Name = name;
            var sum = 0m;
            lootList.ForEach(i =>
            {
                sum += i.Chance;
                i.Sum = sum;
            });
            LootItems = lootList.ToArray();
        }

        /// <summary>
        /// Drop a loot item
        /// </summary>
        /// <returns>Item classname</returns>
        public string Drop()
        {
            var rnd = (decimal)_rnd.NextDouble();
            foreach (var item in LootItems)
            {
                if (item.Sum >= rnd)
                    return item.Item;
            }
            throw new LootException("Erm shouldnt be here... rnd more than 1? C# is broken");
        }
    }
}