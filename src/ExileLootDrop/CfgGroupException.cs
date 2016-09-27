using System;
using System.Runtime.Serialization;

namespace ExileLootDrop
{
    [Serializable]
    public class CfgGroupException : Exception
    {
        public CfgGroupException()
        {
        }

        public CfgGroupException(string message) : base(message)
        {
        }

        public CfgGroupException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CfgGroupException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}