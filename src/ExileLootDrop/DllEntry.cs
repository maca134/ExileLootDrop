using System;
using System.Runtime.InteropServices;
using System.Text;
using RGiesecke.DllExport;

namespace ExileLootDrop
{
    /// <summary>
    /// Entry point class for ARMA
    /// </summary>
    public class DllEntry
    {
        /// <summary>
        /// Entry point method for ARMA
        /// </summary>
        [DllExport("_RVExtension@12", CallingConvention = CallingConvention.Winapi)]
        public static void RvExtension(StringBuilder output, int outputSize, [MarshalAs(UnmanagedType.LPStr)] string function)
        {
            try
            {
                output.Append(Loot.Invoke(function));
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.Level.Error, "Uncaught Exception!");
                Logger.Log(ex);
                output.Append("ERROR");
            } 
        }
    }
}
