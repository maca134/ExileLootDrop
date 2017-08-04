using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ExileLootDrop
{
    public class Loot
    {
        private static bool _hasErrored;

        /// <summary>
        /// Extension path
        /// </summary>
        public static string BasePath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";

        /// <summary>
        /// Max depth for table groups
        /// </summary>
        public static int MaxDepth { get; set; } = 50;

        /// <summary>
        /// Loot tables
        /// </summary>
        public static Loot LootTable { get; } = new Loot();

        /// <summary>
        /// Static method for ARMA's callExtension
        /// </summary>
        /// <param name="input">Input from ARMA</param>
        /// <returns>Results to send back to ARMA</returns>
        public static string Invoke(string input)
        {
            if (_hasErrored)
            {
                Logger.Log<Loot>(Logger.Level.Error, "Trying to run ext after an error has occured!");
                return "ERROR";
            }
            var parts = input.Split('|');
            var table = parts[0];
            var count = (parts.Length == 2) ? int.Parse(parts[1]) : 1;
            if (count > 200)
                count = 200;
            try
            {
                return string.Join("|", LootTable.GetItems(table, count));
            }
            catch (LootTableNotFoundException ex)
            {
                Logger.Log<Loot>(Logger.Level.Error, $"Looks like you tried to get loot from a table that didnt exist: {input}");
                Logger.Log<Loot>(ex);
                return "ERROR";
            }
        }
        
        private readonly List<CfgGroup> _cfgGroups;
        public Dictionary<string, LootTable> Table { get; } = new Dictionary<string, LootTable>();

        /// <summary>
        /// Loot constructor
        /// </summary>
        private Loot()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = Path.GetFileNameWithoutExtension(assembly.Location);
            var iniPath = Path.Combine(BasePath, $"{Regex.Replace(assemblyName, @"_x64$", string.Empty)}.ini");
            if (!File.Exists(iniPath))
                throw new LootException($"{assemblyName}.ini was not found");
            
            var ini = new IniParser(iniPath);
            var cfgpath = ini.GetSetting("General", "LootCfg", "ExileLootDrop.cfg");
            var cacheCount = Convert.ToInt32(ini.GetSetting("General", "CacheItems", "0"));
            
            Logger.LoggerHandlerManager.AddHandler(new ConsoleLoggerHandler());
            var logfile = Path.Combine(BasePath, "output.log");
            try
            {
                if (File.Exists(logfile))
                {
                    File.Delete(logfile);
                }
            }
            catch
            {
                // ignore
            }
            Logger.LoggerHandlerManager.AddHandler(new FileLoggerHandler(logfile));
            Logger.Log<Loot>(Logger.Level.Info, "=====================================================================");
            Logger.Log<Loot>(Logger.Level.Info, "Exile Loot Drop Extension");
            Logger.Log<Loot>(Logger.Level.Info, "By maca134");
            Logger.Log<Loot>(Logger.Level.Info, "Please remember to donate for more cool shit!");
            Logger.Log<Loot>(Logger.Level.Info, "http://maca134.co.uk");
            Logger.Log<Loot>(Logger.Level.Info, "=====================================================================");
            
            var start = DateTime.Now;
            try
            {
                _cfgGroups = CfgGroup.Load(cfgpath);
            }
            catch (CfgGroupException ex)
            {
                _hasErrored = true;
                Logger.Log<Loot>(Logger.Level.Error, $"There was an error loading the loot cfg {cfgpath}: {ex.Message}");
                throw new LootException($"There was an error loading the loot cfg {cfgpath}", ex);
            }
            foreach (var group in _cfgGroups)
            {
                var list = FlattenGroups(group);
                var table = new LootTable(group.Name, list, cacheCount);
                Table.Add(group.Name, table);
            }
            var span = DateTime.Now - start;
            Logger.Log<Loot>($"Took {span.TotalMilliseconds}ms to load and parse loot cfg");
        }
        
        /// <summary>
        /// Flattens cfg groups (recursive)
        /// </summary>
        /// <param name="group">Base group</param>
        /// <param name="depth">Current depth</param>
        /// <returns></returns>
        private List<LootItem> FlattenGroups(CfgGroup group, int depth = 0)
        {
            var list = new List<LootItem>();
            if (depth > MaxDepth)
            {
                Logger.Log<Loot>(Logger.Level.Warning, $"The loot table is too deep: {depth} > {MaxDepth}");
                return list;
            }
            var total = group.Items.Select(i => i.Chance).Sum();
            foreach (var item in group.Items.OrderByDescending(a => a.Chance))
            {
                var chance = item.Chance / total;
                var child = _cfgGroups.Find(g => g.Name == item.Item);
                if (child != null)
                {
                    var childlist = FlattenGroups(child, depth++);
                    childlist.ForEach(i =>
                    {
                        list.Add(new LootItem
                        {
                            Item = i.Item,
                            Chance = i.Chance * chance
                        });
                    });
                    continue;
                }
                list.Add(new LootItem
                {
                    Item = item.Item,
                    Chance = chance
                });
            }
            return list;
        }

        /// <summary>
        /// Drops X amount of loot items from table
        /// </summary>
        /// <param name="table">Loot table name</param>
        /// <param name="count">Amount of items to return</param>
        /// <returns></returns>
        public string[] GetItems(string table, int count = 1)
        {
            if (!Table.ContainsKey(table))
                throw new LootTableNotFoundException($"No loot table called {table}");

            var items = new string[count];
            for (var i = 0; i < count; i++)
                items[i] = Table[table].Drop();

            return items;
        }

        /// <summary>
        /// Get tables
        /// </summary>
        /// <returns>List of table names</returns>
        public string[] GetTables()
        {
            return LootTable.Table.Keys.ToArray();
        }
    }
}