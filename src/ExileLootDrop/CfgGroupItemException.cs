using System;
using System.Runtime.Serialization;

namespace ExileLootDrop
{
    [Serializable]
    public class CfgGroupItemException : CfgGroupException
    {
        public CfgGroupItemException()
        {
        }

        public CfgGroupItemException(string message) : base(message)
        {
        }

        public CfgGroupItemException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CfgGroupItemException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}