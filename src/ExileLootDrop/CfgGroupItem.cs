namespace ExileLootDrop
{
    public class CfgGroupItem
    {
        /// <summary>
        /// Item chance
        /// </summary>
        public int Chance { get; }

        /// <summary>
        /// Item/group name
        /// </summary>
        public string Item { get; }

        /// <summary>
        /// CfgGroupItem constructor
        /// </summary>
        /// <param name="line">Line from the loot config</param>
        public CfgGroupItem(string line)
        {
            var parts = line.Split(',');
            if (parts.Length != 2)
                throw new CfgGroupItemException($"Item line is invalid: {line}");
            int chance;
            if (!int.TryParse(parts[0], out chance))
                throw new CfgGroupItemException($"Could not parse chance: {line}");
            Chance = chance;
            Item = parts[1].Trim();
        }
    }
}