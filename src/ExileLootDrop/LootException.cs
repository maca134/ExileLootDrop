using System;
using System.Runtime.Serialization;

namespace ExileLootDrop
{
    [Serializable]
    internal class LootException : Exception
    {
        public LootException()
        {
        }

        public LootException(string message) : base(message)
        {
        }

        public LootException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LootException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}