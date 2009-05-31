#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2009 Paul Stancer.
// All rights reserved. This program and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html
//
// Contributors:
//     Paul Stancer - initial implementation
// *******************************************************************************
#endregion
#region using
using System;
using System.Text;
using System.IO;
using System.Security;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

using Ximura;
using Ximura.Server;
#endregion // using
namespace Ximura.Helper
{
    /// <summary>
    /// The <b>Common</b> class includes a number of useful utilities.
    /// </summary>
    public static class Common
    {
        #region Command Parsing
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Args"></param>
        /// <returns>Returns a dictionary containing the collection of parameters and values.</returns>
        public static Dictionary<string, string> ParseArgs(string[] Args)
        {
            return ParseArgs(Args, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Args"></param>
        /// <param name="throwErrors"></param>
        /// <returns>Returns a dictionary containing the collection of parameters and values.</returns>
        public static Dictionary<string, string> ParseArgs(string[] Args, bool throwErrors)
        {
            return ParseArgs(Args, @"/", @":", throwErrors);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Args"></param>
        /// <param name="strStart"></param>
        /// <param name="throwErrors"></param>
        /// <returns>Returns a dictionary containing the collection of parameters and values.</returns>
        public static Dictionary<string, string> ParseArgs(string[] Args, string strStart, bool throwErrors)
        {
            return ParseArgs(Args, strStart, @":", throwErrors);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Args"></param>
        /// <param name="strStart"></param>
        /// <param name="strDelim"></param>
        /// <param name="throwErrors"></param>
        /// <returns>Returns a dictionary containing the collection of parameters and values.</returns>
        public static Dictionary<string, string> ParseArgs(string[] Args, string strStart, string strDelim, bool throwErrors)
        {
            //	This function parses the command line arguments to find the correct type
            //	based on the syntax /strOption:[strReturnData]
            //	Although there are more efficent ways, other than a for-next loop,
            //	to search through a list, there will only be a limited number of 
            //	items so this has not been optimized. 

            Dictionary<string, string> data = new Dictionary<string, string>();

            if (Args == null) 
                return data;

            string strKey, strValue;

            foreach (string strData in Args)
            {
                try
                {
                    ParseData(strData, out strKey, out strValue, strStart, strDelim);

                    if (!data.ContainsKey(strKey))
                    {
                        data.Add(strKey, strValue);
                    }
                    else
                    {
                        if (throwErrors) 
                            throw new ArgumentException("Multiple keys found.", strKey);
                    }
                }
                catch
                {
                    if (throwErrors)
                    {
                        //Check the string format
                        throw new ArgumentException("Incorrect format", strData);
                    }
                }


            }

            return data;

        }

        private static void ParseData(string strData, out string strKey,
            out string strValue, string strStart, string strDelim)
        {
            //Ok, trim any space of the data
            strData = strData.Trim();
            //Does the string start with strStart, if not throw an error.
            if (!strData.StartsWith(strStart)) throw new ArgumentException();

            //Is the delimiter of 0 length
            if (strDelim.Length == 0)
            {
                //Just return the key
                strKey = strData.Substring(strStart.Length - 1);
                strValue = "";
            }
            else
            {
                //OK, get the position of the delimiter
                int intDelim = strData.IndexOf(strDelim, strStart.Length);

                if (intDelim == -1)
                {
                    strKey = strData.Substring(strStart.Length);
                    strValue = "";
                }
                else
                {
                    strKey = strData.Substring(strStart.Length, intDelim - strStart.Length);
                    strValue = strData.Substring(intDelim + 1);
                }
            }


        }
        #endregion

        #region MIME and Byte() Functions

        #region Write Bytes Functions

        public static void WriteBytes(MemoryStream Message, string strData)
        {
            byte[] byData = Encoding.ASCII.GetBytes(strData);
            Message.Write(byData, 0, byData.Length);
        }

        public static void WriteBytes(MemoryStream Message, StringBuilder strbData)
        {
            WriteBytes(Message, strbData, true);
        }

        public static void WriteBytes(MemoryStream Message, StringBuilder strbData, bool blnClearSTRB)
        {
            if (strbData.Length > 0)
            {
                byte[] byData = Encoding.ASCII.GetBytes(strbData.ToString());
                Message.Write(byData, 0, byData.Length);
            }
            if (blnClearSTRB) strbData.Length = 0;
        }

        public static void WriteBytes(MemoryStream Message, byte[] byData)
        {
            Message.Write(byData, 0, byData.Length);
        }

        /// <summary>
        /// This converts a string in to an ASCII byte array. This method
        /// adds a CRLF at the end of the string by default.
        /// </summary>
        /// <param name="strData">The string you wish to convert.</param>
        /// <returns>A byte array containing an ASCII representation of the string.</returns>
        public static byte[] ASCByt(string strData)
        {
            return ASCByt(strData, true);
        }
        /// <summary>
        /// This converts a string in to an ASCII byte array.
        /// </summary>
        /// <param name="strData">The string you wish to convert.</param>
        /// <param name="blnAddCRLF">Select true if you want a new line appended at the end.</param>
        /// <returns>A byte array containing an ASCII representation of the string.</returns>
        public static byte[] ASCByt(string strData, bool blnAddCRLF)
        {
            if (blnAddCRLF) return Encoding.ASCII.GetBytes(strData + Environment.NewLine);

            return Encoding.ASCII.GetBytes(strData);
        }

        #endregion

        #region Binary Search Functions

        private static int FindCharCaseInsensitive(byte[] byData,
            byte byteSearch, int intStart)
        {
            return FindCharCaseInsensitive(byData, byteSearch, intStart, false);
        }

        private static int FindCharCaseInsensitive(byte[] byData,
            byte byteSearch, int intStart, bool blnCaseInsensitive)
        {
            if (!blnCaseInsensitive)
                return Array.IndexOf(byData, (byte)byteSearch, intStart);

            //OK, it's a bit more complex as we have to check for a lowercase char
            byte charLow = 0;
            //Encoding.ASCII.GetBytes(Char.ToLower(Convert.ToChar(bySearch)));
            byte charHigh = 0;
            //Encoding.ASCII.GetBytes(Char.ToUpper(Convert.ToChar(bySearch)));

            int intLower = Array.IndexOf(byData, charLow, intStart, 1);
            int intUpper = Array.IndexOf(byData, charHigh, intStart, 1);

            if (intLower < intUpper && intLower > -1)
            {
                return intLower;
            }
            else
            {
                return intUpper;
            }

        }

        public static int BinarySearchExt(byte[] byData,
            byte[] bySearch, int intSearchPosition)
        {
            return BinarySearchExt(byData, bySearch, intSearchPosition, byData.Length, false);
        }

        public static int BinarySearchExt(byte[] byData,
            byte[] bySearch, int intSearchPosition,
            int intLength)
        {
            return BinarySearchExt(byData, bySearch, intSearchPosition, intLength, false);
        }

        public static int BinarySearchExt(byte[] byData,
            byte[] bySearch, int intSearchPosition,
            int intLength, bool blnIgnoreCase)
        {

            if (intSearchPosition < 0)
                throw new ArgumentOutOfRangeException("intSearchPosition", @"Argument must be greater than -1");

            if (intSearchPosition > byData.Length) return -1;

            int intPointer = 0;

            //OK, let's see if we can find the first instance of the first 
            //character of bySearch
            intPointer = FindCharCaseInsensitive(byData,
                bySearch[0], intSearchPosition, blnIgnoreCase);

            //No luck, or has it passed the max length (intLength) that we 
            //should look in?
            if (intPointer < 0 || intPointer > intLength - 1)
                return -1;

            bool blnFailCheck = false;
            while (intPointer >= 0 || intPointer < intLength)
            {
                for (int intLoop = 1; intLoop <= bySearch.Length - 1; intLoop++)
                {
                    if ((intPointer + intLoop) >= intLength)
                    {
                        blnFailCheck = true;
                        break;
                    }

                    if (blnIgnoreCase)
                    {
                        blnFailCheck = blnFailCheck ||
                            (Char.ToLower(Convert.ToChar(bySearch[intLoop])) !=
                            Char.ToLower(Convert.ToChar(byData[intPointer + intLoop])));
                    }
                    else
                    {
                        blnFailCheck = blnFailCheck ||
                            (bySearch[intLoop] != byData[intPointer + intLoop]);
                    }

                    if (blnFailCheck) break;
                }

                if (!blnFailCheck) break;

                //intPointer = Array.IndexOf(byData, bySearch(0), intPointer + 1, intLength - intPointer - 1)
                intPointer = FindCharCaseInsensitive(byData, bySearch[0], intPointer + 1, blnIgnoreCase);

                if (intPointer > -1 || intPointer < intLength)
                    blnFailCheck = false;
            }

            if (blnFailCheck) return -1;

            return intPointer;
        }

        #endregion

        #region Byte Endoding Functions

        #region Hex Encoding


        /// <summary>
        /// This function converts a binary array in to a hexadecimal string.
        /// </summary>
        /// <param name="data">The byte array to convert.</param>
        /// <returns>A string that represents the byte array as hexadecimal</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">This exception 
        /// is thrown if the offset and the length and greater than the 
        /// binary array length.</exception>
        public static string Enc_EncodeByteToHex(byte[] data)
        {
            return Enc_EncodeByteToHex(data, 0, data.Length, false);
        }
        /// <summary>
        /// This function converts a binary array in to a hexadecimal string.
        /// </summary>
        /// <param name="data">The byte array to convert.</param>
        /// <param name="offset">The starting point in the byte array.</param>
        /// <param name="length">The number of bytes to convert.</param>
        /// <returns>A string that represents the byte array as hexadecimal</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">This exception 
        /// is thrown if the offset and the length and greater than the 
        /// binary array length.</exception>
        public static string Enc_EncodeByteToHex(byte[] data, int offset, int length, bool lower)
        {
            if (data.Length < offset + length)
                throw new ArgumentOutOfRangeException("length", length, "Length is too long.");

            StringBuilder output = new StringBuilder(length * 2);
            string id;
            if (lower)
                id = "x2";
            else
                id = "X2";

            for (int loopthru = 0; loopthru < length; loopthru++)
            {
                output.Append(data[offset + loopthru].ToString(id));
            }
            return output.ToString();

        }
        #endregion

        #region Hex Decoding

        /// <summary>
        /// This method decodes a hex string and inserts it into a byte array.
        /// </summary>
        /// <param name="HexString">The hex string that you wish to parse.</param>
        /// <param name="data">The byte array that you wish to be updated</param>
        /// <param name="offset">The point within the byte array that the method should start inserting encoded data.</param>
        /// <param name="length">The number of decoded bytes that the method should insert in the byte array.</param>
        /// <exception cref="System.ArgumentException">This exception is thrown if the hex 
        /// string is not divisible by 2. All strings should be left padded with 0 to ensure
        /// that they are divisible by 2.</exception>
        public static void Enc_DecodeHexAsByte(string HexString,
            byte[] data, int offset, int length)
        {
            if ((HexString.Length % 2) > 0)
                throw new ArgumentException("The Hex string must be divisible by 2");

            for (int strPoint = 0; strPoint < length; strPoint++)
            {
                byte temp = byte.Parse(HexString.Substring(strPoint * 2, 2),
                    NumberStyles.HexNumber);

                data[strPoint + offset] = temp;
            }
        }
        /// <summary>
        /// This method decodes a hex string and returns it as a byte array.
        /// </summary>
        /// <param name="HexString">The hex string to decode.</param>
        /// <returns>A byte array containing the bytes corresponding to the decoded string.</returns>
        /// <exception cref="System.ArgumentException">This exception is thrown if the hex 
        /// string is not divisible by 2. All strings should be left padded with 0 to ensure
        /// that they are divisible by 2.</exception>
        public static byte[] Enc_DecodeHexString(string HexString)
        {
            if ((HexString.Length % 2) > 0)
                throw new ArgumentException("The Hex string must be divisible by 2");

            int length = HexString.Length / 2;
            byte[] data = new byte[length];

            Enc_DecodeHexAsByte(HexString, data, 0, length);
            return data;
        }

        #endregion

        #endregion

        #endregion

        #region Security Function

        #region Declaration of Crytography Field
        /// <summary>
        /// Enumeration list for hash algorithms
        /// </summary>
        public enum HashAlgorithmType
        {
            //MD5 Algorithm
            MD5,
            //SHA1Managed Algoritm
            SHA1
        }
        #endregion

        #region Crytography Hash Function
        /// <summary>
        /// Converting a hex string to base 64 string
        /// </summary>
        /// <param name="HexString"></param>
        /// <returns></returns>
        public static string ConvertHexStringToBase64(string HexString)
        {
            return Convert.ToBase64String(Enc_DecodeHexString(HexString));
        }

        /// <summary>
        /// Converts a Base64 string in to a hex string.
        /// </summary>
        /// <param name="base64String">The base64 string.</param>
        /// <returns>The hex string.</returns>
        public static string ConvertBase64ToHexString(string base64String)
        {
            return Enc_EncodeByteToHex(Convert.FromBase64String(base64String));
        }

        /// <summary>
        /// Converting a string to byte[] using UTF8 encoding
        /// </summary>
        /// <param name="stringToHash"></param>
        /// <returns></returns>
        public static byte[] ConvertStringToByte(string stringToHash)
        {
            //Create a new instance of UTF8Encoding to 
            //convert the string into an array of UTF8 bytes.
            UTF8Encoding UTF8 = new UTF8Encoding();

            //Convert the string into an array of bytes.

            return UTF8.GetBytes(stringToHash);
        }

        /// <summary>
        /// Function to hash a string by default hash algorithm (MD5)
        /// </summary>
        /// <param name="stringToHash">String to hash</param>
        /// <returns>Hashed string</returns>
        public static string ComputeHash(string stringToHash)
        {
            return ComputeHash(stringToHash, HashAlgorithmType.MD5);
        }

        /// <summary>
        /// Function to hash a string by specific hash algorithm
        /// </summary>
        /// <param name="stringToHash">String to hash</param>
        /// <param name="hashType">Hash Algorithm Type</param>
        /// <returns>Hashed string</returns>
        public static string ComputeHash(string stringToHash, HashAlgorithmType hashType)
        {
            //Create a new instance of UTF8Encoding to 
            //convert the string into an array of UTF8 bytes.
            UTF8Encoding UTF8 = new UTF8Encoding();

            //Convert the string into an array of bytes.
            byte[] MessageBytes = UTF8.GetBytes(stringToHash);

            return Convert.ToBase64String(ComputeHash(MessageBytes, hashType));
        }

        /// <summary>
        /// Function to hash a byte[] by default hash algorithm (MD5) 
        /// </summary>
        /// <param name="HashValue">Byte[] to hash</param>
        /// <returns>Hashed byte[]</returns>
        public static byte[] ComputeHash(byte[] HashValue)
        {
            return ComputeHash(HashValue, HashAlgorithmType.MD5);
        }

        /// <summary>
        /// Function to hash a byte[] by specific hash algorithm
        /// </summary>
        /// <param name="HashValue">Byte[] to hash</param>
        /// <param name="hashType">Hash Algorithm Type</param>
        /// <returns>Hashed byte[]</returns>
        public static byte[] ComputeHash(byte[] HashValue, HashAlgorithmType hashType)
        {
            HashAlgorithm mhash = null;

            //Create a new instance of MD5CryptoServiceProvider to create 
            //the hash value.
            switch (hashType)
            {
                case HashAlgorithmType.MD5:
                    mhash = new MD5CryptoServiceProvider();
                    break;
                case HashAlgorithmType.SHA1:
                    mhash = new SHA1Managed();
                    break;
            }

            //Create the hash value from the array of bytes.
            HashValue = mhash.ComputeHash(HashValue);

            return HashValue;
        }

        /// <summary>
        /// This method compares two byte arrays and returns true is the two are equal.
        /// </summary>
        /// <param name="Array1"></param>
        /// <param name="Array2"></param>
        /// <returns></returns>
        public static bool CompareByteArray(byte[] Array1, byte[] Array2)
        {
            //First check whether the 2 byte arrays are of equal length.
            if (Array1.Length != Array2.Length)
                return false;

            //Loop through the 2 arrays and check whether each byte is the same.
            for (int loop = 0; loop < Array1.Length; loop++)
            {
                if (Array1[loop] != Array2[loop])
                    return false;
            }

            //OK, everything seems fine so return true
            return true;
        }

        /// <summary>
        /// This method concatenates two byte arrays and returns the concatenation.
        /// </summary>
        /// <param name="Array1"></param>
        /// <param name="Array2"></param>
        /// <returns></returns>
        public static byte[] ConcatenateByteArray(byte[] Array1, byte[] Array2)
        {
            if (Array1 == null || Array2 == null)
            {
                return null;
            }
            // create the result with the sum of the length of both arrays
            byte[] Result = new byte[Array1.Length + Array2.Length];
            //copy Array1
            Array1.CopyTo(Result, 0);
            //copy Array2
            Array2.CopyTo(Result, Array1.Length);

            return Result;
        }

        /// <summary>
        /// Create a salted password given the salt value.
        /// </summary>
        /// <param name="seedValue">The seed as a byte array</param>
        /// <param name="unsaltedPassword">The password as a string</param>
        /// <returns>A hash byte array</returns>
        public static byte[] CreateSeedHash(string seedValue, string unsaltedPassword)
        {
            byte[] bySeedValue = ConvertStringToByte(seedValue);
            byte[] byPassword = ConvertStringToByte(unsaltedPassword);

            return CreateSeedHash(bySeedValue, byPassword);
        }
        /// <summary>
        /// Create a salted password given the salt value.
        /// </summary>
        /// <param name="seedValue">The seed as a byte array</param>
        /// <param name="unsaltedPassword">The password as a UTF-8 byte array</param>
        /// <returns>A hash byte array</returns>
        public static byte[] CreateSeedHash(byte[] seedValue, byte[] unsaltedPassword)
        {
            // Add the salt to the hash.
            byte[] rawSalted = new byte[unsaltedPassword.Length + seedValue.Length];
            unsaltedPassword.CopyTo(rawSalted, 0);
            seedValue.CopyTo(rawSalted, unsaltedPassword.Length);

            byte[] saltedPassword = ComputeHash(rawSalted);

            return saltedPassword;
        }

        public static string MD5HashPassword(string seedValue, string password)
        {
            return Convert.ToBase64String(CreateSeedHash(seedValue, password));
        }

        public static string GeneratePasswordSeed()
        {
            byte[] HashValue = ComputeHash(Guid.NewGuid().ToByteArray());

            // only returns the 1st 10 characters
            return Convert.ToBase64String(HashValue).Substring(0, 10);
        }

        #endregion

        #region Password protected DataEncryption

        public static string GetHexEncString(Guid AppID, string username, string realm, byte[] hashedTextInBytes)
        {
            byte[] buf = GetEncData(AppID, username, realm, hashedTextInBytes);
            return Enc_EncodeByteToHex(buf);
        }

        public static string GetHexDecString(Guid AppID, string username, string realm, string EncText)
        {
            byte[] buf = GetDecData(AppID, username, realm, Enc_DecodeHexString(EncText));
            return Enc_EncodeByteToHex(buf);
        }

        public static byte[] GetEncData(Guid AppID, string username, string realm, byte[] bufRead)
        {
            Rfc2898DeriveBytes pwdGen =
                new Rfc2898DeriveBytes(username + ":" + realm.ToLowerInvariant(), AppID.ToByteArray(), 1000);

            RijndaelManaged rjn = new RijndaelManaged();

            using (ICryptoTransform eTransform = rjn.CreateEncryptor(pwdGen.GetBytes(32), pwdGen.GetBytes(16)))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, eTransform, CryptoStreamMode.Write))
                    {
                        int value = bufRead.Length;
                        cs.WriteByte((byte)value);
                        cs.WriteByte((byte)(value >> 8));
                        cs.WriteByte((byte)(value >> 0x10));
                        cs.WriteByte((byte)(value >> 0x18));
                        cs.Write(bufRead, 0, value);
                        cs.FlushFinalBlock();

                        return ms.ToArray();
                    }
                }
            }
        }

        public static byte[] GetDecData(Guid AppID, string username, string realm, byte[] bufRead)
        {
            Rfc2898DeriveBytes pwdGen =
                new Rfc2898DeriveBytes(username + ":" + realm.ToLowerInvariant(), AppID.ToByteArray(), 1000);

            RijndaelManaged rjn = new RijndaelManaged();

            using (ICryptoTransform eTransform = rjn.CreateDecryptor(pwdGen.GetBytes(32), pwdGen.GetBytes(16)))
            {
                using (MemoryStream ms = new MemoryStream(bufRead))
                {
                    using (CryptoStream cs = new CryptoStream(ms, eTransform, CryptoStreamMode.Read))
                    {
                        int len = (((cs.ReadByte() | (cs.ReadByte() << 8)) | (cs.ReadByte() << 0x10)) | (cs.ReadByte() << 0x18));

                        byte[] buf = new byte[len];
                        cs.Read(buf, 0, buf.Length);

                        return buf;
                    }
                }
            }
        }
        #endregion // Password protected DataEncryption

        #region Digest Authentication
        #region SelectHashAlgorithm(string hashType)
        /// <summary>
        /// This method selects the appropriate hash algorithm based on the value submitted.
        /// </summary>
        /// <param name="hashType">The hash algorithm identifier.</param>
        /// <returns>Returns the selected hash algorithm.</returns>
        public static HashAlgorithm SelectHashAlgorithm(string hashType)
        {
            if (hashType == null)
                throw new ArgumentNullException("SelectHashAlgorithm: hashType cannot be null.");

            switch (hashType.ToLowerInvariant())
            {
                default:
                    return new MD5CryptoServiceProvider();
            }
        }
        #endregion // SelectHashAlgorithm(string hashType)

        #region HA1Calculate
        public static string HA1Calculate(string hashType, string username, string realm, string password)
        {
            return HA1Calculate(hashType, username, realm, password, null, null);
        }

        public static string HA1Calculate(string hashType, string username, string realm, string password,
            string nonce, string cnonce)
        {
            if (hashType == null)
                hashType = "md5";

            byte[] blobHA1 = null;

            using (HashAlgorithm hash = SelectHashAlgorithm(hashType))
            {
                blobHA1 = hash.ComputeHash(Encoding.UTF8.GetBytes(username + ":" + realm + ":" + password));
            }

            string HA1 = Enc_EncodeByteToHex(blobHA1, 0, blobHA1.Length, true);

            if (hashType.ToLowerInvariant() == "md5-sess")
            {
                return HA1CalculateMD5Sess(hashType, HA1, nonce, cnonce);
            }

            return HA1;
        }

        public static string HA1CalculateMD5Sess(string hashType, string HA1, string nonce, string cnonce)
        {
            if (hashType == null)
                hashType = "md5";

            if (hashType.ToLowerInvariant() == "md5-sess")
            {
                if (nonce == null)
                    throw new ArgumentNullException("nonce cannot be null when the algorithm in set to md5-sess");
                if (cnonce == null)
                    throw new ArgumentNullException("cnonce cannot be null when the algorithm in set to md5-sess");

                byte[] blobHA1 = null;

                using (HashAlgorithm hash = SelectHashAlgorithm(hashType))
                {
                    blobHA1 = hash.ComputeHash(Encoding.UTF8.GetBytes(HA1 + ":" + nonce + ":" + cnonce));
                }

                HA1 = Enc_EncodeByteToHex(blobHA1, 0, blobHA1.Length, true);
            }

            return HA1;
        }
        #endregion // HA1Calculate

        #region HA2Calculate
        public static string HA2Calculate(string hashType, string verb, string uri)
        {
            return HA2Calculate(hashType, verb, uri, null);
        }

        public static string HA2Calculate(string hashType, string verb, string uri, byte[] body)
        {
            if (verb == null)
                throw new ArgumentNullException("verb cannot be null.");
            if (uri == null)
                throw new ArgumentNullException("uri cannot be null.");

            string data = verb + ":" + uri;

            byte[] bHash = null;

            using (HashAlgorithm hash = SelectHashAlgorithm(hashType))
            {
                if (body == null || body.Length == 0)
                    bHash = hash.ComputeHash(Encoding.UTF8.GetBytes(data));
                else
                {
                    bHash = new byte[hash.HashSize / 8];
                    data += ":";
                    byte[] blob1 = Encoding.ASCII.GetBytes(data);
                    hash.TransformBlock(blob1, 0, blob1.Length, bHash, 0);

                    bHash = hash.TransformFinalBlock(body, 0, body.Length);
                }
            }

            return Enc_EncodeByteToHex(bHash, 0, bHash.Length, true);
        }
        #endregion // HA2Calculate

        #region DigestResponseCalculate
        public static string DigestResponseCalculate(string hashType, string HA1, string HA2, string nonce)
        {
            return DigestResponseCalculate(hashType, HA1, HA2, nonce, null, null, null);
        }

        public static string DigestResponseCalculate(string hashType, string HA1, string HA2, string nonce,
            string qop, string nc, string cnonce)
        {
            if (HA1 == null || HA1 == "")
                throw new ArgumentOutOfRangeException("HA1 cannot be null or empty.");
            if (HA2 == null || HA2 == "")
                throw new ArgumentOutOfRangeException("HA2 cannot be null or empty.");
            if (nonce == null || nonce == "")
                throw new ArgumentOutOfRangeException("nonce cannot be null or empty.");


            string data = HA1 + ":" + nonce;

            if (qop != null)
            {
                data += ":" + nc + ":" + cnonce + ":" + qop;
            }

            data += ":" + HA2;

            byte[] outData;

            using (HashAlgorithm hash = SelectHashAlgorithm(hashType))
            {
                outData = hash.ComputeHash(Encoding.ASCII.GetBytes(data));
            }

            return Enc_EncodeByteToHex(outData, 0, outData.Length, true);
        }
        #endregion // DigestResponseCalculate
        #endregion // Digest Authentication

        #endregion

        #region Card Number Helper Function
        /// <summary>
        /// Card Number is obsured in the manner 41124342******33.
        /// </summary>
        /// <param name="strCardNO"></param>
        /// <returns></returns>
        public static string GetCardNoObsured(string strCardNO)
        {
            //card number and expiry date are seperated by a special 
            //char '=' in ASCI
            int cardNOLength = strCardNO.Trim().Length;
            //obsure the card nunmber
            return strCardNO.Substring(0, cardNOLength - 8) + "******" +
                strCardNO.Substring(cardNOLength - 2, 2);
        }


        /// <summary>
        /// This function hash the card number 
        /// </summary>
        /// <param name="strCardNO">Card Number</param>
        /// <returns>card hashed number as string</returns>
        public static string GetCardHash(string strCardNO)
        {
            int cardNOLength = strCardNO.Trim().Length;
            //obsure the card nunmber
            return ComputeHash(strCardNO.Substring(0, cardNOLength));
        }

        /// <summary>
        /// This function hash the card number according to the Card Encoding Attribute
        /// </summary>
        /// <param name="htCardEncodingAttribute">Card Encoding Attribute</param>
        /// <param name="strCardNO">Card Number</param>
        /// <returns>card hashed number as string</returns>
        public static string GetCardHash(Hashtable htCardEncodingAttribute, string strCardNO)
        {
            string CardHashNumber = null;
            if (!(strCardNO == string.Empty || strCardNO == null))
            {
                string CardImportType = htCardEncodingAttribute["card_import_type"] as string;
                string CardEncodingType = htCardEncodingAttribute["card_encoding_type"] as string;
                switch (CardImportType)
                {
                    case "plain-text":
                        CardHashNumber = GetCardHash(strCardNO);
                        break;
                    case "hash":
                        switch (CardEncodingType)
                        {
                            case "hex":
                                CardHashNumber = ConvertHexStringToBase64(strCardNO);
                                break;
                            case "base64":
                                CardHashNumber = strCardNO;
                                break;
                            default:
                                throw new ArgumentException("Invalid Card Encoding Type: '" + CardEncodingType + "'");
                        }
                        break;
                    default:
                        throw new ArgumentException("Invalid Card Import Type: '" + CardImportType + "'");
                }
            }
            return CardHashNumber;
        }
        /// <summary>
        /// This function hash the card number according to the Card Encoding Attribute
        /// </summary>
        /// <param name="strCardImportType">Card Import Type</param>
        /// <param name="strCardEncodingType">Card Encoding Type</param>
        /// <param name="strCardNO">Card Number</param>
        /// <returns>card hashed number as string</returns>
        public static string GetCardHash(string strCardImportType, string strCardEncodingType, string strCardNO)
        {
            string CardHashNumber = null;
            if (!(strCardNO == string.Empty || strCardNO == null))
            {
                switch (strCardImportType)
                {
                    case "plain-text":
                        CardHashNumber = GetCardHash(strCardNO);
                        break;
                    case "hash":
                        switch (strCardEncodingType)
                        {
                            case "hex":
                                CardHashNumber = ConvertHexStringToBase64(strCardNO);
                                break;
                            case "base64":
                                CardHashNumber = strCardNO;
                                break;
                            default:
                                throw new ArgumentException("Invalid Card Encoding Type: '" + strCardEncodingType + "'");
                        }
                        break;
                    default:
                        throw new ArgumentException("Invalid Card Import Type: '" + strCardImportType + "'");
                }
            }
            return CardHashNumber;
        }

        #endregion

        #region Convert Date String to ISO 8601 DateTime Format
        ///// <summary>
        ///// Function to convert string to ISO8601 datetime string yyyy-MM-ddTHH:mm:ss
        ///// </summary>
        ///// <param name="strDateTime">string in special format ( yyyyMMdd | yyyyMMddHH | yyyyMMddHHmm | yyyyMMddHHmmss</param>		
        ///// <returns>ISO8601 datetime string</returns>
        //public static string ConvertToISO8601DateString(string strDateTime)
        //{
        //    StringBuilder strDate = new StringBuilder();

        //    strDate.Append(strDateTime.Substring(0,4) + "-"); //append the year 
        //    strDate.Append(strDateTime.Substring(4,2) + "-"); //append the month
        //    strDate.Append(strDateTime.Substring(6,2) + "T"); //append the day

        //    switch (strDateTime.Length)
        //    {
        //        case 8:		//format yyyyMMdd 
        //            strDate.Append("00:00:00");					
        //            break;
        //        case 10:		//format yyyyMMddHH
        //            strDate.Append(strDateTime.Substring(8,2) + ":00:00");
        //            break;
        //        case 12:		//format yyyyMMddHHmm 
        //            strDate.Append(strDateTime.Substring(8,2) + ":");
        //            strDate.Append(strDateTime.Substring(10,2) + ":00");							
        //            break;
        //        case 14:		//format yyyyMMddHHmmss 
        //            strDate.Append(strDateTime.Substring(8,2) + ":");
        //            strDate.Append(strDateTime.Substring(10,2) + ":");
        //            strDate.Append(strDateTime.Substring(12,2));	
        //            break;
        //        default:
        //            return "";
        //    }		

        //    return strDate.ToString();
        //}
        #region ConvertToRFC1123DateString
        /// <summary>
        /// This method converts a datetime parameter to an RFC1123 string formar.
        /// </summary>
        /// <param name="dt">The date time.</param>
        /// <returns>The string representation.</returns>
        public static string ConvertToRFC1123DateString(DateTime dt)
        {
            return dt.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss") + " GMT";
        }
        #endregion // ConvertToRFC1123DateString
        #region ToRFC1123String(this DateTime dt)
        /// <summary>
        /// This method converts a datetime parameter to an RFC1123 string formar.
        /// </summary>
        /// <param name="dt">The date time.</param>
        /// <returns>The string representation.</returns>
        public static string ToRFC1123String(this DateTime dt)
        {
            return dt.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss") + " GMT";
        }
        #endregion // ToRFC1123String(this DateTime dt)


        /// <summary>
        /// Function to convert string to ISO8601 datetime string yyyy-MM-ddTHH:mm:ss
        /// </summary>
        /// <param name="strDateTime">string in special format ( yyyyMMdd | yyyyMMddHH | yyyyMMddHHmm | yyyyMMddHHmmss</param>		
        /// <returns>ISO8601 datetime string</returns>
        public static string ConvertToISO8601DateString(string strDateTime)
        {
            if (strDateTime.Length < 8)
                return "";

            string strDate = strDateTime.Substring(0, 4) + "-"; //append the year 
            strDate += strDateTime.Substring(4, 2) + "-"; //append the month
            strDate += strDateTime.Substring(6, 2) + "T"; //append the day

            switch (strDateTime.Length)
            {
                case 8:		//format yyyyMMdd 
                    strDate += "00:00:00";
                    break;
                case 10:		//format yyyyMMddHH
                    strDate += strDateTime.Substring(8, 2) + ":00:00";
                    break;
                case 12:		//format yyyyMMddHHmm 
                    strDate += strDateTime.Substring(8, 2) + ":";
                    strDate += strDateTime.Substring(10, 2) + ":00";
                    break;
                case 14:		//format yyyyMMddHHmmss 
                    strDate += strDateTime.Substring(8, 2) + ":";
                    strDate += strDateTime.Substring(10, 2) + ":";
                    strDate += strDateTime.Substring(12, 2);
                    break;
                default:
                    return "";
            }

            return strDate;
        }

        public static string ConvertToISO8601DateStringWithOffset(DateTime dtDateTime)
        {
            string offsetStr = "Z";
            TimeSpan offset = TimeZone.CurrentTimeZone.GetUtcOffset(dtDateTime);
            if (offset != TimeSpan.Zero)
            {
                if (offset.Hours < 0)
                    offsetStr = "-";
                else
                    offsetStr = "+";
                offsetStr += offset.Hours.ToString("00") + ":" + offset.Minutes.ToString("00");
            }
            return ConvertToISO8601DateString(dtDateTime) + offsetStr;
        }
        /// <summary>
        /// Function to convert datetime to ISO8601 datetime string yyyy-MM-ddTHH:mm:ss
        /// </summary>
        /// <param name="dtDateTime">datetime</param>		
        /// <returns>ISO8601 datetime string</returns>
        public static string ConvertToISO8601DateString(DateTime dtDateTime)
        {
            StringBuilder sbDateTime = new StringBuilder();

            sbDateTime.Append(dtDateTime.Year.ToString().PadLeft(4, '0') + "-"); //append the year 
            sbDateTime.Append(dtDateTime.Month.ToString().PadLeft(2, '0') + "-"); //append the month
            sbDateTime.Append(dtDateTime.Day.ToString().PadLeft(2, '0') + "T"); //append the day
            sbDateTime.Append(dtDateTime.Hour.ToString().PadLeft(2, '0') + ":"); //append the hour
            sbDateTime.Append(dtDateTime.Minute.ToString().PadLeft(2, '0') + ":"); //append the minute
            sbDateTime.Append(dtDateTime.Second.ToString().PadLeft(2, '0')); //append the second

            return sbDateTime.ToString();
        }
        /// <summary>
        /// This method adjusts the incoming 6 digit date to a 8 digit date with the century included.
        /// </summary>
        /// <param name="strDate">date</param>		
        /// <returns>8-digit string</returns>
        public static string AdjustCentury(string strDate)
        {
            if (strDate.Length != 6)
                return strDate;

            return GuessCentury(int.Parse(strDate.Substring(0, 2))).ToString() + strDate;
        }
        /// <summary>
        /// Guess the Year based on the Month and the Current Year
        /// </summary>
        /// <param name="Month">Month</param>
        /// <returns>Year</returns>
        public static int GuessYear(int Month)
        {
            int currMonth = DateTime.Now.Month;

            if (Month < 0 || Month > 12)
                throw new ArgumentOutOfRangeException();
            else if (currMonth < Month)
                return DateTime.Now.Year - 1;
            else
                return DateTime.Now.Year;
        }

        /// <summary>
        /// Guess the Century of the 2-digit Year, calculated based on the current century +/- 50 years
        /// </summary>
        /// <param name="Year">Year</param>
        /// <returns>Century</returns>
        public static int GuessCentury(int Year)
        {
            DateTime today = DateTime.Today;
            int currCentury;
            int currYear;

            currYear = (int)decimal.Remainder(today.Year, 100);
            currCentury = (today.Year - currYear) / 100;

            if (Math.Abs(currYear - Year) < 50)
            {
                return currCentury;
            }
            else if (currYear < Year)
            {
                return currCentury - 1;
            }
            else
            {
                return currCentury + 1;
            }
        }
        public static DateTime ConvertAgeToBirthday(int age)
        {
            // set to the Jan 1
            DateTime birthday = new DateTime(DateTime.Today.Year - age, 1, 1);
            return birthday;
        }

        public static Decimal StringDateToAge(string dateValue)
        {
            int year = 0;
            try
            {

                year = DateTime.Now.Year - DateTime.Parse(dateValue).Year;

            }
            catch
            {
                return 0;
            }
            return year;
        }

        #endregion
        #region Convert Expiry Date
        public static DateTime ConvertExpiryDate(int expiryYear, int expiryMonth)
        {
            DateTime expiryDate = DateTime.MinValue;

            // guess the century
            if (expiryYear < 100)
            {
                expiryYear += (GuessCentury(expiryYear) * 100);
            }
            try
            {
                // set to the start of the month (1st day of the month)
                expiryDate = new DateTime(expiryYear, expiryMonth, 1);
                // set it to the last day of the month by adding 1 month and subtract 1 day
                expiryDate = expiryDate.AddMonths(1).AddDays(-1);
            }
            catch
            {
                throw new ArgumentException("Expiry Date is not date time");
            }
            return expiryDate;
        }
        #endregion

        #region DataGridValueMapper
        /// <summary>
        /// This class provide common data grid value mapper.
        /// </summary>
        public class DataGridValueMapper
        {
            protected static Dictionary<string, string> trueFalse2YesNoValueMapper = new Dictionary<string, string>();
            static DataGridValueMapper()
            {
                trueFalse2YesNoValueMapper["True"] = "Yes";
                trueFalse2YesNoValueMapper["False"] = "No";
            }
            public static Dictionary<string, string> TrueFalse2YesNo { get { return trueFalse2YesNoValueMapper; } }
        }
        #endregion

        #region HTTP Codes
        /// <summary>
        /// This class provides shortcuts to the HTTP codes.
        /// </summary>
        public class HTTPCodes
        {
            /// <summary>
            /// 100: Continue
            /// </summary>
            public static string Continue_100 { get { return "100"; } }
            /// <summary>
            /// 101: Switching Protocols
            /// </summary>
            public static string SwitchingProtocols_101 { get { return "101"; } }

            /// <summary>
            /// 200: OK
            /// </summary>
            public static string OK_200 { get { return "200"; } }
            /// <summary>
            /// 201: Created
            /// </summary>
            public static string Created_201 { get { return "201"; } }
            /// <summary>
            /// 202: Accepted
            /// </summary>
            public static string Accepted_202 { get { return "202"; } }
            /// <summary>
            /// 203: Non Authoritative
            /// </summary>
            public static string NonAuthoritative_203 { get { return "203"; } }
            /// <summary>
            /// 204: No Content
            /// </summary>
            public static string NoContent_204 { get { return "204"; } }
            /// <summary>
            /// 205: Reset Content
            /// </summary>
            public static string ResetContent_205 { get { return "205"; } }
            /// <summary>
            /// 206: Partial Content
            /// </summary>
            public static string PartialContent_206 { get { return "206"; } }

            /// <summary>
            /// 300: Multiple Choices
            /// </summary>
            public static string MultipleChoices_300 { get { return "300"; } }
            /// <summary>
            /// 301: Moved Permanently
            /// </summary>
            public static string MovedPermanently_301 { get { return "301"; } }
            /// <summary>
            /// 302: Found
            /// </summary>
            public static string Found_302 { get { return "302"; } }
            /// <summary>
            /// 303: See Other
            /// </summary>
            public static string SeeOther_303 { get { return "303"; } }
            /// <summary>
            /// 304: Not Modified
            /// </summary>
            public static string NotModified_304 { get { return "304"; } }
            /// <summary>
            /// 305: Use Proxy
            /// </summary>
            public static string UseProxy_305 { get { return "305"; } }
            /// <summary>
            /// 306: Redirection Command Not Used
            /// </summary>
            public static string ReDirectionCommandNotUsed_306 { get { return "306"; } }
            /// <summary>
            /// 307: Moved Temporarily
            /// </summary>
            public static string MovedTemporarily_307 { get { return "307"; } }

            /// <summary>
            /// 400: Bad Request
            /// </summary>
            public static string BadRequest_400 { get { return "400"; } }
            /// <summary>
            /// 401: Unauthorized
            /// </summary>
            public static string Unauthorized_401 { get { return "401"; } }
            /// <summary>
            /// 402: Payment Required
            /// </summary>
            public static string PaymentRequired_402 { get { return "402"; } }
            /// <summary>
            /// 403: Forbidden
            /// </summary>
            public static string Forbidden_403 { get { return "403"; } }
            /// <summary>
            /// 404: Not Found
            /// </summary>
            public static string NotFound_404 { get { return "404"; } }
            /// <summary>
            /// 405: Method Not Allowed
            /// </summary>
            public static string MethodNotAllowed_405 { get { return "405"; } }
            /// <summary>
            /// 406: Not Acceptable
            /// </summary>
            public static string NotAcceptable_406 { get { return "406"; } }
            /// <summary>
            /// 407: Proxy Authentication Required
            /// </summary>
            public static string ProxyAuthenticationRequired_407 { get { return "407"; } }
            /// <summary>
            /// 408: Request Timeout
            /// </summary>
            public static string RequestTimeout_408 { get { return "408"; } }
            /// <summary>
            /// 409: Conflict
            /// </summary>
            public static string Conflict_409 { get { return "409"; } }
            /// <summary>
            /// 410: Gone
            /// </summary>
            public static string Gone_410 { get { return "410"; } }
            /// <summary>
            /// 411: Length Required
            /// </summary>
            public static string LengthRequired_411 { get { return "411"; } }
            /// <summary>
            /// 412: Precondition Failed
            /// </summary>
            public static string PreconditionFailed_412 { get { return "412"; } }
            /// <summary>
            /// 413: Request Entity Too Large
            /// </summary>
            public static string RequestEntityTooLarge_413 { get { return "413"; } }
            /// <summary>
            /// 414: Request URI Too Long
            /// </summary>
            public static string RequestURITooLong_414 { get { return "414"; } }
            /// <summary>
            /// 415: Unsupported Media Type
            /// </summary>
            public static string UnsupportedMediaType_415 { get { return "415"; } }
            /// <summary>
            /// 416: Requested Range Not Satisfiable
            /// </summary>
            public static string RequestedRangeNotSatisfiable_416 { get { return "416"; } }
            /// <summary>
            /// 417: Expectation Failed
            /// </summary>
            public static string ExpectationFailed_417 { get { return "417"; } }

            /// <summary>
            /// 500: Internal Server Error
            /// </summary>
            public static string InternalServerError_500 { get { return "500"; } }
            /// <summary>
            /// 501: Not Implemented
            /// </summary>
            public static string NotImplemented_501 { get { return "501"; } }
            /// <summary>
            /// 502: Bad Gateway
            /// </summary>
            public static string BadGateway_502 { get { return "502"; } }
            /// <summary>
            /// 503: Service Unavailable
            /// </summary>
            public static string ServiceUnavailable_503 { get { return "503"; } }
            /// <summary>
            /// 504: Gateway Timeout
            /// </summary>
            public static string GatewayTimeout_504 { get { return "504"; } }
            /// <summary>
            /// 505: Version No tSupported
            /// </summary>
            public static string VersionNotSupported_505 { get { return "505"; } }

            #region IsHTTPError(string status)
            /// <summary>
            /// This method returns true if the status is an HTTP error.
            /// </summary>
            /// <param name="status">The status to check.</param>
            /// <returns>Returns true if the status code denotes an error.</returns>
            public static bool IsHTTPError(string status)
            {
                return IsServerError(status) || status.StartsWith("4", true, CultureInfo.InvariantCulture);
            }
            #endregion // IsHTTPError(string status)
            #region IsServerError(string status)
            /// <summary>
            /// This method returns true if the status code is an error.
            /// </summary>
            /// <param name="status">The status to check.</param>
            /// <returns>Returns true if the status code denotes an error.</returns>
            public static bool IsServerError(string status)
            {
                return status.StartsWith("5", true, CultureInfo.InvariantCulture);
            }
            #endregion // IsServerError(string status)
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// This method tests whether a string is numeric.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(string value)
        {
            if (value == null || value == "")
                return false;

            double tryIt;
            return double.TryParse(value, out tryIt);
        }

        /// <summary>
        /// This method tests whether a string is a DateTime.
        /// </summary>
        /// <param name="dateValue">The date value.</param>
        /// <returns>Returns true if the string is a date.</returns>
        public static bool IsDateTime(string dateValue)
        {
            return IsDateTime(dateValue, "000000");
        }
        /// <summary>
        /// This method tests whether a string is a DateTime.
        /// </summary>
        /// <param name="dateValue">The date value.</param>
        /// <param name="timeValue">The time value.</param>
        /// <returns>Returns true if the string is a date.</returns>
        public static bool IsDateTime(string dateValue, string timeValue)
        {
            DateTime dtN;
            return DateTime.TryParse(ConvertToISO8601DateString(dateValue + timeValue), out dtN);
        }
        /// <summary>
        /// This method tests whether a string is a DateTime.
        /// </summary>
        /// <param name="dateValue">The date value.</param>
        /// <param name="timeValue">The time value.</param>
        /// <returns>Returns true if the string is a date.</returns>
        public static bool IsDateTime(string dateValue, string timeValue, out DateTime? dtN)
        {
            dtN = null;
            if (dateValue == string.Empty)
                return false;

            if (timeValue == string.Empty)
                return false;

            DateTime dt;
            bool success = DateTime.TryParse(ConvertToISO8601DateString(dateValue + timeValue), out dt);

            if (success)
                dtN = dt;

            return success;
        }

        #endregion // Helper Methods

        #region CreatePasswordHash
        /// <summary>
        /// This method creates a password hash from a byte64 encoded text seed and plain text password
        /// </summary>
        /// <param name="Seed"></param>
        /// <param name="Pass"></param>
        /// <returns></returns>
        public static string CreatePasswordHash(string Seed, string Pass)
        {
            byte[] seed = System.Text.UTF8Encoding.UTF8.GetBytes(Seed);

            byte[] pass = System.Text.UTF8Encoding.UTF8.GetBytes(Pass);

            byte[] rawSalted = new byte[pass.Length + seed.Length];
            pass.CopyTo(rawSalted, 0);
            seed.CopyTo(rawSalted, pass.Length);

            HashAlgorithm mhash = new MD5CryptoServiceProvider();

            //Create the hash value from the array of bytes.
            byte[] HashValue = mhash.ComputeHash(rawSalted);
            return Convert.ToBase64String(HashValue);
        }
        #endregion // CreatePasswordHash

        #region SplitOnChars
        /// <summary>
        /// This method is used to split string pairs.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="toSplit"></param>
        /// <param name="convertT"></param>
        /// <param name="convertU"></param>
        /// <param name="split1"></param>
        /// <param name="split2"></param>
        /// <returns></returns>
        public static List<KeyValuePair<T, U>> SplitOnChars<T, U>(string toSplit,
            Converter<string, T> convertT, Converter<string, U> convertU,
                char[] split1, char[] split2)
        {
            if (toSplit == null)
                throw new ArgumentNullException("toSplit", "toSplit cannot be null.");

            List<KeyValuePair<T, U>> newList = new List<KeyValuePair<T, U>>();

            string[] pairs = toSplit.Split(split1, StringSplitOptions.RemoveEmptyEntries);

            if (pairs.Length == 0)
                return newList;

            foreach (string pair in pairs)
            {
                string[] pairSplit = pair.Split(split2);
                string secondParam = pairSplit.Length == 1 ? null : pairSplit[1];
                KeyValuePair<T, U> keyPair =
                    new KeyValuePair<T, U>(convertT(pairSplit[0]), convertU(secondParam));
                newList.Add(keyPair);
            }

            return newList;
        }
        #endregion // SplitOnChars
        #region SplitOnCharsUnique
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="toSplit"></param>
        /// <param name="convertT"></param>
        /// <param name="convertU"></param>
        /// <param name="split1"></param>
        /// <param name="split2"></param>
        /// <returns></returns>
        public static Dictionary<T, U> SplitOnCharsUnique<T, U>(string toSplit,
            Converter<string, T> convertT, Converter<string, U> convertU, char[] split1, char[] split2)
        {
            if (toSplit == null)
                throw new ArgumentNullException("toSplit", "toSplit cannot be null.");

            Dictionary<T, U> newList = new Dictionary<T, U>();

            string[] pairs = toSplit.Split(split1, StringSplitOptions.RemoveEmptyEntries);

            if (pairs.Length == 0)
                return newList;

            foreach (string pair in pairs)
            {
                string[] pairSplit = pair.Split(split2);
                string secondParam = pairSplit.Length == 1 ? null : pairSplit[1];
                newList.Add(convertT(pairSplit[0]), convertU(secondParam));
            }

            return newList;
        }
        #endregion // SplitOnCharsUnique

        #region ConvPassthru
        public static Converter<string, string> ConvPassthru = 
            delegate(string input) 
                { 
                    return input; 
                };
        #endregion // ConvPassthru
        #region ConvPassthruLowerCase
        public static Converter<string, string> ConvPassthruLowerCase = 
            delegate(string input) 
                { 
                    return input.Trim().ToLowerInvariant(); 
                };
        #endregion // ConvPassthruLowerCase
        #region ConvQParam
        public static Converter<string, string> ConvQParam = 
            delegate(string input)
                {
                    if (input == null)
                        return null;
                    input = input.Trim().ToLower();
                    if (!input.StartsWith("q="))
                        return null;
                    return input.Substring(2);
                };
        #endregion // ConvQParam
        #region ConvStripSpeechMarks
        public static Converter<string, string> ConvStripSpeechMarks = 
            delegate(string input)
                {
                    if (input == null)
                        return null;
                    if (input == @"""""")
                        return "";
                    if (!input.StartsWith(@"""") || !input.EndsWith(@"""") || input.Length < 2)
                        return input;
                    return input.Substring(1, input.Length - 2);
                };
        #endregion // ConvStripSpeechMarks
    }
}

