using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ExileLootDrop
{
    /// <summary>
    /// Class to represent each Loot group in a config file
    /// </summary>
    public class CfgGroup
    {
        private const string BlockComments = @"/\*(.*?)\*/";
        private const string LineComments = @"//[^\n]+";

        /// <summary>
        /// Load a loot config into a List of CfgGroups
        /// </summary>
        /// <param name="file">Path to loot config. If relative will start in the same directory as this extension</param>
        /// <returns>CfgGroup List</returns>
        public static List<CfgGroup> Load(string file)
        {
            if (!Path.IsPathRooted(file))
            {
                var basepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (basepath != null)
                    file = Path.Combine(basepath, file);
            }
            if (!File.Exists(file))
                throw new CfgGroupException($"File not found: {file}");
            var lootcfg = Regex.Replace(File.ReadAllText(file),
                BlockComments + "|" + LineComments,
                "\n",
                RegexOptions.Singleline);
            var groups = new List<CfgGroup>();
            CfgGroup group = null;
            foreach (var line in lootcfg.Split('\r', '\n').Where(l => !string.IsNullOrWhiteSpace(l)))
            {
                if (line[0] == '>')
                {
                    group = new CfgGroup(line.Substring(1).Trim());
                    groups.Add(group);
                    continue;
                }
                group?.Add(line);
            }
            return groups;
        }
        /// <summary>
        /// Name of the group
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// List of loot items
        /// </summary>
        public List<CfgGroupItem> Items { get; } = new List<CfgGroupItem>();

        /// <summary>
        /// CfgGroup constructor
        /// </summary>
        /// <param name="name">Group name</param>
        public CfgGroup(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Add a loot item
        /// </summary>
        /// <param name="line">Line from the loot config</param>
        internal void Add(string line)
        {
            try
            {
                Items.Add(new CfgGroupItem(line));
            }
            catch (CfgGroupItemException ex)
            {
                throw new CfgGroupException($"Failed to add GroupItem: {line}", ex);
            }
        }
    }
}