#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2010 Paul Stancer.
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
#endregion // using
namespace Ximura
{
    /// <summary>
    /// The <b>Common</b> class includes a number of useful utilities.
    /// </summary>
    public static partial class Common
    {
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

    }
}
