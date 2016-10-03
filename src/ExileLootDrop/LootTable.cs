using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly int _cacheCount;
        private int _cachePtr;
        private readonly LootItem[] _cacheItems;

        /// <summary>
        /// LootTable constuctor
        /// </summary>
        /// <param name="name">Table name</param>
        /// <param name="lootList">Item list</param>
        public LootTable(string name, List<LootItem> lootList, int cacheCount = 0)
        {
            _cacheCount = cacheCount;
            Name = name;
            var sum = 0m;
            lootList.ForEach(i =>
            {
                sum += i.Chance;
                i.Sum = sum;
            });
            LootItems = lootList.ToArray();
            Logger.Log<LootTable>($"Pre caching loot for {Name}");
            var cache = new List<LootItem>();
            for (var i = 0; i < _cacheCount; i++)
            {
                var rnd = (decimal)_rnd.NextDouble();
                foreach (var item in LootItems.Where(item => item.Sum >= rnd))
                {
                    cache.Add(item);
                    break;
                }
            }
            _cachePtr = 0;
            _cacheItems = cache.ToArray();
        }

        /// <summary>
        /// Drop a loot item
        /// </summary>
        /// <returns>Item classname</returns>
        public string Drop()
        {
            if (_cacheCount != 0 && _cachePtr < _cacheCount)
            {
                return _cacheItems[_cachePtr++].Item;
            }

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