using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExileLootDrop;

namespace ExileLootDropTester
{
    internal class Program
    {
        private const int Loops = 50000;

        private static void Main(string[] args)
        {
            Console.WriteLine("Loading Loot");
            //Loot.LootTable
            Console.WriteLine();

            var tables = Loot.LootTable.GetTables();
            if (args.Length > 0 && args[0] == "dump")
            {
                File.WriteAllText(
                    Path.Combine(Environment.CurrentDirectory, "dump.txt"),
                    $"[\"{string.Join("\",\"", tables.Select(table => string.Join("\",\"", Loot.LootTable.Table[table].LootItems.Select(l => l.Item))))}\"]"
                );
                return;
            }
            for (var i = 0; i < tables.Length; i++)
            {
                var table = tables[i];
                Console.WriteLine($"{i}: {table}");
            }
            Console.WriteLine();
            Console.WriteLine("Select a table: ");
            var input = Console.ReadLine();
            Console.WriteLine();
            if (!int.TryParse(input, out int index))
            {
                Console.WriteLine("input error");
                return;
            }
            if (index >= tables.Length)
            {
                Console.WriteLine("input out of range");
                return;
            }
            var selectedTable = tables[index];
            Console.WriteLine($"\"{selectedTable}\" selected");
            Console.WriteLine($"Calculating Loot - Running {Loops} iterations...");
            Console.WriteLine();

            var lootdrops = new Dictionary<string, int>();
            var start = DateTime.Now;
            for (var i = 0; i < Loops - 1; i++)
            {
                var item = Loot.Invoke(selectedTable);
                if (!lootdrops.ContainsKey(item))
                    lootdrops[item] = 0;
                lootdrops[item]++;
            }
            var timetaken = DateTime.Now - start;
            var items = from pair in lootdrops
                        orderby pair.Value descending
                        select pair;
            
            foreach (var pair in items)
            {
                Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
            }
            Console.WriteLine();
            Console.WriteLine($"Took {timetaken.TotalMilliseconds}ms to do {Loops} items - {timetaken.TotalMilliseconds / Loops}ms per item");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
