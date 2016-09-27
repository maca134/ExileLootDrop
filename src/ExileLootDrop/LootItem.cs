namespace ExileLootDrop
{
    public class LootItem
    {
        /// <summary>
        /// Item classname
        /// </summary>
        public string Item { get; set; }

        /// <summary>
        /// Chance for item
        /// </summary>
        public decimal Chance { get; set; }

        /// <summary>
        /// Cumulative
        /// </summary>
        public decimal Sum { get; set; }
    }
}