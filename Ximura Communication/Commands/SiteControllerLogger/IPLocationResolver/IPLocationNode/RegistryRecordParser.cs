#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using RH = Ximura.Reflection;
using AH = Ximura.AttributeHelper;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This structure holds the address and mask binary data for the address.
    /// </summary>
    public struct RegistryRecordParser
    {
        /// <summary>
        /// This is the IP address as a byte array.
        /// </summary>
        public readonly byte[] Data;
        /// <summary>
        /// This is the corresponding netmask as a byte array.
        /// </summary>
        public readonly byte[] Mask;

        #region Constructor
        /// <summary>
        /// This constructor converts both the IPAddress and mask in to a byte array.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="netMask">the net mask.</param>
        public RegistryRecordParser(IPAddress address, string netMask)
        {
            Data = address.GetAddressBytes();

            switch (address.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    Mask = ConvertMaskIPV4(netMask);
                    break;
                case AddressFamily.InterNetworkV6:
                    Mask = ConvertMaskIPV6(netMask);
                    break;
                default:
                    throw new NotSupportedException("Only InterNetwork and InterNetworkV6 are supported for the address.");
            }
        }
        #endregion // Constructor

        #region GetLevel(int level, out byte data, out byte mask)
        /// <summary>
        /// This method retrieves the specific bytes for the address and mask for the level specified.
        /// </summary>
        /// <param name="level">The level, i.e. byte position in the array.</param>
        /// <param name="data">The data byte.</param>
        /// <param name="mask">The mask byte.</param>
        public void GetLevel(int level, out byte data, out byte mask)
        {
            if (level > Data.Length - 1)
                throw new ArgumentOutOfRangeException("level", "level is out of range.");

            data = Data[level];
            mask = Mask[level];
        }
        #endregion // GetLevel(int level, out byte data, out byte mask)
        #region LevelOK(int level)
        /// <summary>
        /// This property specifies whether the level is valid for the address.
        /// </summary>
        /// <param name="level">The level value.</param>
        /// <returns>Returns true if there is data for this level.</returns>
        public bool LevelOK(int level)
        {
            return level < Data.Length;
        }
        #endregion // LevelOK(int level)

        #region ConvertMaskIPV4(string netMask)
        /// <summary>
        /// This method converts the mask for the IPV4 records.
        /// </summary>
        /// <param name="netMask">The mask value.</param>
        /// <returns>Returns a byte array containing the mask bytes.</returns>
        private static byte[] ConvertMaskIPV4(string netMask)
        {
            uint value;
            if (!uint.TryParse(netMask, out value))
                throw new ArgumentException(string.Format("netMask is not a valid value: {0}", netMask),"netMask");

            value = uint.MaxValue - value + 1;

            byte[] buffer = new byte[4];

            buffer[3] = (byte)value;
            buffer[2] = (byte)(value >> 8);
            buffer[1] = (byte)(value >> 0x10);
            buffer[0] = (byte)(value >> 0x18);

            return buffer;
        }
        #endregion // ConvertMaskIPV4(string netMask)
        #region ConvertMaskIPV6(string netMask)
        /// <summary>
        /// This method converts the mask for the IPV6 records.
        /// </summary>
        /// <param name="netMask">The mask value.</param>
        /// <returns>Returns a byte array containing the mask bytes.</returns>
        private static byte[] ConvertMaskIPV6(string netMask)
        {
            throw new NotImplementedException("IPv6 is not implemented");
            //uint value;
            //if (!uint.TryParse(netMask, out value))
            //    throw new ArgumentException(string.Format("netMask is not a valid value: {0}", netMask), "netMask");

            //value = uint.MaxValue - value + 1;

            //byte[] buffer = new byte[16];

            //buffer[3] = (byte)value;
            //buffer[2] = (byte)(value >> 8);
            //buffer[1] = (byte)(value >> 0x10);
            //buffer[0] = (byte)(value >> 0x18);

            //return buffer;
        }
        #endregion // ConvertMaskIPV4(string netMask)
    }
}
