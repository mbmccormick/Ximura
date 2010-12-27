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
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Diagnostics;
using System.Security.Cryptography;
using System.IO;
using System.IO.Compression;
using System.Text;

using Ximura;

using Ximura.Data;
using CH = Ximura.Common;
#endregion
namespace Ximura.Communication
{
    /// <summary>
    /// This is the default message class for reading and writing large binary blobs to a file based storage medium.
    /// </summary>
    public class FileBasedStorageMessage : Message, IXimuraBinarySerialize
    {
        #region Declarations
        private Stream mDataStream;

        private Stream mFileStream;
        private Stream mEncrStream;
        private Stream mCompStream;


        private bool mEncrypted;
        private Rijndael mRijndaelAlg = null;

        private bool mHashed;
        private byte[] mDataHash = null;

        private DateTime? mCreateTime;
        private bool mCompressed;
        private DirectoryInfo mDataStoreLocation;
        private Guid? mMessageID;

        private object syncStreamObj = new object();
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public FileBasedStorageMessage()
            : base()
        {
        }
        #endregion
        #region Reset()
        /// <summary>
        /// This is the reset method to set the content.
        /// </summary>
        public override void Reset()
        {
            if (mDataStream != null)
                mDataStream.Close();
            mDataStream = null;

            mFileStream = null;

            mCompressed = true;
            mCompStream = null;

            mEncrypted = false;
            mEncrStream = null;
            mRijndaelAlg = null;

            mHashed = true;
            mDataHash = null;

            mCreateTime = null;
            mDataStoreLocation = null;
            mMessageID = null;
            base.Reset();
        }
        #endregion // Reset()

        #region Load(DirectoryInfo dataStoreLocation, Guid messageID)
        /// <summary>
        /// This method loads the message from an existing store ready for reading.
        /// </summary>
        /// <param name="dataStoreLocation">The data store location.</param>
        /// <param name="messageID">The sepcific message ID.</param>
        public virtual void Load(DirectoryInfo dataStoreLocation, Guid messageID)
        {
            if (Loaded)
                throw new MessageLoadException("The message is already loaded.");
            if (Initializing)
                throw new MessageLoadException("The message is in the initialization phase.");

            if (!dataStoreLocation.Exists)
                throw new IOException("Folder '" + dataStoreLocation.FullName + "' does not exist.");

            mDataStoreLocation = dataStoreLocation;
            mMessageID = messageID;
            //Load the metadata from the file store.
            MetadataRead();

            mDataStream = DataStreamRead();
            Load();
        }
        #endregion // Load(DirectoryInfo dataStoreLocation, Guid messageID)
        #region Load(long MaxSize, DirectoryInfo dataStoreLocation, Guid messageID)
        /// <summary>
        /// This message creates a new message ready for writing at the location specified.
        /// </summary>
        /// <param name="MaxSize">The maximum permissible size of the message.</param>
        /// <param name="messageID">The message ID.</param>
        /// <param name="dataStoreLocation">The data store location. If the location does not exist</param>
        public virtual void Load(long MaxSize, DirectoryInfo dataStoreLocation, Guid messageID)
        {
            if (Loaded)
                throw new MessageLoadException("The message is already loaded.");
            if (Initializing)
                throw new MessageLoadException("The message is in the initialization phase.");

            if (!dataStoreLocation.Exists)
                throw new IOException("Folder '" + dataStoreLocation.FullName + "' does not exist.");

            mDataStoreLocation = dataStoreLocation;
            mMessageID = messageID;

            mDataStream = DataStreamWrite();
            Load(MaxSize);
        }
        #endregion // Load(long MaxSize, DirectoryInfo dataStoreLocation, Guid messageID)

        #region Close()
        /// <summary>
        /// This override persists the metainformation for the stream.
        /// </summary>
        public override void Close()
        {
            MetadataWrite();
            base.Close();
        }
        #endregion // Close()

        #region MetadataWrite()
        /// <summary>
        /// This method saves the message metadata to a file stream.
        /// </summary>
        protected virtual void MetadataWrite()
        {
            if (File.Exists(MetadataFilename))
                throw new IOException("The file already exists.");

            MemoryStream mStream = new MemoryStream();
            BinaryWriter w = new BinaryWriter(mStream, Encoding.UTF8);
            Write(w);

            byte[] buffer = new byte[mStream.Length];

            mStream.Position = 0;
            mStream.Read(buffer, 0, buffer.Length);

            byte[] data = ProtectedData.Protect(buffer,
                MessageID.Value.ToByteArray(), DataProtectionScope.LocalMachine);

            File.WriteAllBytes(MetadataFilename, data);
            //File.WriteAllBytes(MetadataFilename, buffer);
        }
        #endregion // MetadataWrite()
        #region MetadataRead()
        /// <summary>
        /// This method reads the message metadata from the file stream.
        /// </summary>
        protected virtual void MetadataRead()
        {
            if (!File.Exists(MetadataFilename))
                throw new IOException("The dat file cannot be found.");

            byte[] buffer = File.ReadAllBytes(MetadataFilename);

            byte[] data = ProtectedData.Unprotect(buffer,
                MessageID.Value.ToByteArray(), DataProtectionScope.LocalMachine);

            Read(new BinaryReader(new MemoryStream(data)));
        }
        #endregion // MetadataRead()

        #region FragmentSetNext(Type fragmentType, int maxLength)
        /// <summary>
        /// This method returns a new fragment object for the type specified.
        /// </summary>
        /// <param name="fragmentType">The fragment type required.</param>
        /// <param name="maxLength">The maximum permitted length for the fragment.</param>
        protected override IXimuraMessage FragmentSetNext(Type fragmentType, long maxLength)
        {
            lock (this)
            {
                IXimuraMessage fragment = this.PoolGetObject(fragmentType) as IXimuraMessage;

                IXimuraMessageFileStreamLoad fileFragment = fragment as IXimuraMessageFileStreamLoad;
                if (fileFragment == null)
                    throw new ArgumentNullException(fragmentType.FullName + " cannot convert to IXimuraMessageFileStreamLoad.");
                //Set the maximum length of the fragment.
                fileFragment.Load((long)maxLength, mDataStream);
                FragmentAddInternal(fragment);

                return fragment;
            }
        }
        #endregion

        #region ValidateFolderPath(string dataStoreLocation)
        /// <summary>
        /// This method checks the folder location path. If the path does not exist, the path is created.
        /// </summary>
        /// <param name="dataStoreLocation">The data store location.</param>
        /// <returns>Returns the DirectoryInfo for the location.</returns>
        protected virtual DirectoryInfo ValidateFolderPath(string dataStoreLocation)
        {
            DirectoryInfo dInfo;

            if (!Directory.Exists(dataStoreLocation))
                dInfo = Directory.CreateDirectory(dataStoreLocation);
            else
                dInfo = new DirectoryInfo(dataStoreLocation);

            return dInfo;
        }
        #endregion // ValidateFolderPath(string dataStoreLocation)

        #region Hashed
        /// <summary>
        /// This property identifies whether the body data is hashed.
        /// </summary>
        protected virtual bool Hashed
        {
            get
            {
                return mHashed;
            }
        }
        #endregion // Encrypted
        #region Encrypted
        /// <summary>
        /// This property identifies whether the body data is encrypted.
        /// </summary>
        protected virtual bool Encrypted
        {
            get
            {
                return mEncrypted;
            }
        }
        #endregion // Encrypted
        #region Compressed
        /// <summary>
        /// This property identifies whether the body data is compressed.
        /// </summary>
        protected virtual bool Compressed
        {
            get
            {
                return mCompressed;
            }
        }
        #endregion // Compressed

        #region RijndaelAlg
        /// <summary>
        /// This is the encryption key used to protect the base data stream information.
        /// </summary>
        protected virtual Rijndael RijndaelAlg
        {
            get
            {
                return mRijndaelAlg;
            }            
            set
            {
                mRijndaelAlg = value;
            }
        }
        #endregion // RijndaelAlg
        
        #region DataStreamWrite()
        /// <summary>
        /// This method returns a writeable data stream.
        /// </summary>
        /// <returns></returns>
        protected virtual Stream DataStreamWrite()
        {
            Stream outStream = null; ;
            RijndaelAlg = new RijndaelManaged();

            try
            {
                mFileStream = new FileStream(BinaryDataFileName, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                outStream = mFileStream;

                if (Encrypted)
                {
                    mEncrStream = new CryptoStream(outStream, RijndaelAlg.CreateEncryptor(), CryptoStreamMode.Write);
                    outStream = mEncrStream;
                }

                if (Compressed)
                {
                    mCompStream = new GZipStream(outStream, CompressionMode.Compress);
                    outStream = mCompStream;
                }

                outStream = new StreamCounter(outStream);
            }
            catch (Exception ex)
            {
                //Just make sure that we don't leave anything open in case there is an error.
                if (outStream != null)
                    outStream.Close();

                mFileStream = null;
                mEncrStream = null;
                mCompStream = null;
            }

            return outStream;
        }
        #endregion // DataStreamWrite()
        #region DataStreamRead()
        /// <summary>
        /// This method returns a readable data stream.
        /// </summary>
        /// <returns></returns>
        protected virtual Stream DataStreamRead()
        {
            Stream outStream;

            outStream = new FileStream(BinaryDataFileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            if (Encrypted)
                outStream = new CryptoStream(outStream, RijndaelAlg.CreateDecryptor(), CryptoStreamMode.Read);

            if (Compressed)
                outStream = new GZipStream(outStream, CompressionMode.Decompress);

            outStream = new StreamCounter(outStream, StreamCounter.CounterDirection.Read, Length);

            return outStream;
        }
        #endregion // DataStreamRead()

        #region IXimuraBinarySerialize Members
        #region Read(BinaryReader r)
        /// <summary>
        /// This method reads the metadata from the metadata stream.
        /// </summary>
        /// <param name="r">The binary reader for the stream.</param>
        public virtual void Read(BinaryReader r)
        {
            mEncrypted = r.ReadBoolean();

            if (Encrypted)
            {
                byte[] mRijndaelAlgKey, mRijndaelAlgIV;
                ReadVal(r, out mRijndaelAlgKey);
                ReadVal(r, out mRijndaelAlgIV);
                RijndaelAlg = new RijndaelManaged();
                RijndaelAlg.Key = mRijndaelAlgKey;
                RijndaelAlg.IV = mRijndaelAlgIV;
            }

            mHashed = r.ReadBoolean();
            if (Hashed)
            {
                ReadVal(r, out mDataHash);
            }

            mCompressed = r.ReadBoolean();
            Length = r.ReadInt64();

            ReadVal(r, out mCreateTime);
        }
        #endregion // Read(BinaryReader r)
        #region Write(BinaryWriter logWriter)
        /// <summary>
        /// This method writes the metadata for the message to the stream.
        /// </summary>
        /// <param name="logWriter">The binary writer for the metadata stream.</param>
        public virtual void Write(BinaryWriter logWriter)
        {
            logWriter.Write(Encrypted);

            if (Encrypted)
            {
                WriteVal(logWriter, RijndaelAlg.Key);
                WriteVal(logWriter, RijndaelAlg.IV);
            }

            logWriter.Write(Compressed);
            logWriter.Write(Length);

            WriteVal(logWriter, mCreateTime);
        }
        #endregion // Write(BinaryWriter logWriter)
        #endregion

        #region MessageID
        /// <summary>
        /// This is the unique message ID
        /// </summary>
        public virtual Guid? MessageID
        {
            get
            {
                return mMessageID;
            }
        }
        #endregion // MessageID

        #region MetadataFilename
        /// <summary>
        /// This property is the filename for the metadata.
        /// </summary>
        public virtual string MetadataFilename
        {
            get
            {
                return FilenameBase + ".dat";
            }
        }
        #endregion // MetadataFilename
        #region BinaryDataFileName
        /// <summary>
        /// This property is the file name for the binary data.
        /// </summary>
        public virtual string BinaryDataFileName
        {
            get
            {
                return FilenameBase + ".bin";
            }
        }
        #endregion // BinaryDataFileName
        #region FileStoreDirectory
        /// <summary>
        /// This is the file store location base.
        /// </summary>
        protected virtual string FileStoreDirectory
        {
            get
            {
                if (mDataStoreLocation == null)
                    throw new ArgumentNullException("DataStoreLocation is null.");

                string path = mDataStoreLocation.FullName.Replace(@"/", @"\");

                return path.EndsWith(@"\") ? mDataStoreLocation.FullName : mDataStoreLocation.FullName + @"\";
            }
        }
        #endregion // FileStoreDirectory
        #region FilenameBase
        /// <summary>
        /// This is the filename base.
        /// </summary>
        protected virtual string FilenameBase
        {
            get
            {
                if (!MessageID.HasValue)
                    throw new ArgumentNullException("MessageID is null.");

                return FileStoreDirectory + MessageID.Value.ToString("N").ToUpperInvariant();
            }
        }
        #endregion // FilenameBase

        #region CompletionCheck()
        /// <summary>
        /// This override specifies the completion check for the message.
        /// </summary>
        /// <returns>Returns true if the message has completed.</returns>
        protected override bool CompletionCheck()
        {
            
            switch (this.Direction)
            {
                case MessageDirection.Write:
                    return FragmentCurrent != null && !FragmentCurrent.CanWrite;
                case MessageDirection.Read:
                    return FragmentCurrent != null && !FragmentCurrent.CanRead;
                default:
                    throw new NotImplementedException("CompletionCheck is not implemented for this Direction value.");
            }
        }
        #endregion // CompletionCheck()


#if DEBUG
        public override int Write(byte[] buffer, int offset, int count)
        {
            return base.Write(buffer, offset, count);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
 	         return base.Read(buffer, offset, count);
        }
#endif
    }
}
