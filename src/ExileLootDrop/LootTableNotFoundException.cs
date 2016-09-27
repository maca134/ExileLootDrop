using System;
using System.Runtime.Serialization;

namespace ExileLootDrop
{
    [Serializable]
    internal class LootTableNotFoundException : LootException
    {
        public LootTableNotFoundException()
        {
        }

        public LootTableNotFoundException(string message) : base(message)
        {
        }

        public LootTableNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LootTableNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}