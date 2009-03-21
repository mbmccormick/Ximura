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
﻿#region using
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Security.Cryptography;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using Ximura.Server;
using Ximura.Command;
using AH = Ximura.Helper.AttributeHelper;
using RH = Ximura.Helper.Reflection;
using CH = Ximura.Helper.Common;
#endregion
namespace Ximura.Server
{
    public partial class AppServer<CONFSYS, CONFCOM, PERF>
    {
        #region Declarations
        /// <summary>
        /// This attribute contains the command configuration settings.
        /// </summary>
        protected XimuraAppServerConfigCommandAttribute mAttrConfigCommand = null;
        /// <summary>
        /// This attribute contains the system configuration settings.
        /// </summary>
        protected XimuraAppServerConfigSystemAttribute mAttrConfigSystem = null;
        #endregion // Declarations

        #region ConfigurationAttributesGet()
        /// <summary>
        /// This method gets the app server configuration attributes.
        /// </summary>
        protected virtual void ConfigurationAttributesGet()
        {
            mAttrConfigSystem = GetType().Attribute<XimuraAppServerConfigSystemAttribute>();

            mAttrConfigCommand = GetType().Attribute<XimuraAppServerConfigCommandAttribute>();
        }
        #endregion // ConfigurationAttributesGet()

        #region ConfigurationStart()
        /// <summary>
        /// This protected method checks whether the configuration service is active, 
        /// and if not creates a default one.
        /// </summary>
        protected override bool ConfigurationStart()
        {
            //Get the server configuration attributes.
            ConfigurationAttributesGet();

            //Create the system configuration
            ConfigurationSystem = new CONFSYS();

            bool success = ConfigurationSystemLoad(ConfigurationSystem);

            if (!success)
                return false;

            //Process the command configuration.
            success = base.ConfigurationStart();

            return success;
        }
        #endregion
        #region ConfigurationStop()
        /// <summary>
        /// This method stops the configuration manager.
        /// </summary>
        protected override void ConfigurationStop()
        {
            //We wait until here to start the services as they have reference to themselves.
            ((IXimuraService)ConfigurationManager).Stop();
            ConfigurationManager = null;

            base.ConfigurationStop();
        }
        #endregion

        #region ConfigurationSystem
        /// <summary>
        /// The system configuration object.
        /// </summary>
        protected virtual CONFSYS ConfigurationSystem { get; set; }
        #endregion // ConfigurationSystem

        #region ConfigurationLoadGeneric
        /// <summary>
        /// This method loads the configuration based on the attribute settings.
        /// </summary>
        /// <param name="config">The configuration to load.</param>
        /// <param name="configAttr">The attribute settings.</param>
        /// <param name="delDefaultStream">The delegate to the default stream.</param>
        /// <returns>Returns true if the configuration was loaded successfully.</returns>
        protected virtual bool ConfigurationLoadGeneric(ConfigurationBase config,
            XimuraAppServerConfigurationAttribute configAttr, Func<Stream> delDefaultStream)
        {
            using (Stream configStream = LoadConfigFile(configAttr, delDefaultStream))
            {
                if (configStream == null)
                    return false;

                return config.Load(configStream) > 0;
            }
        }
        #endregion // ConfigurationLoadGeneric

        #region ConfigurationLoad(CONFCOM commandConfiguration)
        /// <summary>
        /// This override loads the server command configuration.
        /// </summary>
        /// <param name="commandConfiguration">The command configuration.</param>
        /// <returns>Returns true if the configuration was loaded successfully.</returns>
        protected override bool ConfigurationLoad(CONFCOM commandConfiguration)
        {
            return ConfigurationLoadGeneric(commandConfiguration, mAttrConfigCommand, ConfigurationDefault);
        }
        #endregion // ConfigurationLoad(CONFCOM commandConfiguration)
        #region ConfigurationDefault()
        /// <summary>
        /// This method returns the default command configuration as a stream.
        /// </summary>
        /// <returns></returns>
        protected virtual Stream ConfigurationDefault()
        {
            return RH.ResourceLoadFromUriAsStream(new Uri(
                "xmrres://Ximura/Ximura.Server.AppServer/Ximura.Server.Configuration.Command.AppServerCommandConfiguration_Default.xml"));
        }
        #endregion // ConfigurationDefault()

        #region ConfigurationSystemLoad(CONFSYS systemConfiguration)
        /// <summary>
        /// This method loads the system configuration.
        /// </summary>
        /// <param name="systemConfiguration">The system configuration.</param>
        /// <returns>Returns true if the configuration was loaded successfully.</returns>
        protected virtual bool ConfigurationSystemLoad(CONFSYS systemConfiguration)
        {
            return ConfigurationLoadGeneric(systemConfiguration, mAttrConfigSystem, ConfigurationSystemDefault);
        }
        #endregion // ConfigurationSystemLoad(CONFSYS systemConfiguration)
        #region ConfigurationSystemDefault()
        /// <summary>
        /// This method returns the default system configuration as a stream.
        /// </summary>
        /// <returns></returns>
        protected virtual Stream ConfigurationSystemDefault()
        {
            return RH.ResourceLoadFromUriAsStream(new Uri(
                "xmrres://Ximura/Ximura.Server.AppServer/Ximura.Server.Configuration.System.AppServerSystemConfiguration_Default.xml"));
        }
        #endregion // protected virtual Stream ConfigurationSystemDefault()


        #region LoadConfigFile() method and helper methods
        /// <summary>
        /// This method loads and verifies the application settings for the 
        /// application. This method can be overriden by derived classes.
        /// </summary>
        protected virtual Stream LoadConfigFile(XimuraAppServerConfigurationAttribute attr, Func<Stream> delDefaultStream)
        {
            try
            {
                if (attr == null)
                    return delDefaultStream();

                //Find and process the appropriate settings configuration...
                switch (attr.ConfigOptions)
                {
                    //If there no application settings file for this application, get out of here
                    case AppServerConfigOptions.NoConfig:
                        return delDefaultStream();

                    case AppServerConfigOptions.ResourceStream:
                        return LoadConfigFromAssembly(attr.ConfigLocation, true);
                    //OK, this is a unencrypted file stream, so we will load the stream and pass control
                    //to the LoadConfigFromFile method

                    case AppServerConfigOptions.File:
                    case AppServerConfigOptions.FileDigitallySigned:
                        return LoadConfigFromFile(attr.ConfigLocation,
                            attr.ConfigOptions == AppServerConfigOptions.FileDigitallySigned);

                    case AppServerConfigOptions.FileEncrypted:
                        return DecryptAndLoadConfigFromFile(attr.ConfigLocation);

                    default:
                        throw new ArgumentOutOfRangeException("AppServerConfigOptions",
                            attr.ConfigOptions, "Unexpected AppServerConfigOptions value");
                }
            }
            catch (Exception ex)
            {
                throw new AppServerException(@"The configuration file """ + attr.ConfigLocation + @""" failed to load.", ex);
            }
        }

        /// <summary>
        /// This method loads a config file from a Assembly
        /// </summary>
        /// <param name="resourceFileName">The file name of the config file.</param>
        private Stream LoadConfigFromAssembly(string resourceFileName)
        {
            return LoadConfigFromAssembly(resourceFileName, true);
        }
        /// <summary>
        /// This method loads a config file from a Assembly
        /// </summary>
        /// <param name="resourceFileName">The file name of the config file.</param>
        /// <param name="SearchLoadedAssemblies">A boolean value indicating whether the server 
        /// should search loaded assemblies should be searched if the resource cannot be found
        /// in the current assembly.</param>
        private Stream LoadConfigFromAssembly(string resourceFileName, bool SearchLoadedAssemblies)
        {
            Stream strmConfig = this.GetType().Assembly.GetManifestResourceStream(resourceFileName);

            if (strmConfig == null && SearchLoadedAssemblies)
            {
                //Stream is still null let's try the loaded assemblies instead
                foreach (Assembly theAssembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        strmConfig = theAssembly.GetManifestResourceStream(resourceFileName);
                    }
                    catch { }
                    if (strmConfig != null)
                        break;
                }
            }

            if (strmConfig == null)
                throw new FileLoadException("Resource File stream cannot be found in the Assembly", resourceFileName);

            return strmConfig;
        }

        private Stream getResourceFromAssembly(Assembly theAssembly, string resourceFileName)
        {
            return theAssembly.GetManifestResourceStream(resourceFileName);
        }

        /// <summary>
        /// This method loads a config file from known application paths
        /// </summary>
        /// <param name="fileName">The file name of the config file.</param>
        /// <param name="verifySignature">A boolean value indicating whether the system should 
        /// verify the file content using a digital signature</param>
        private Stream LoadConfigFromFile(string fileName, bool verifySignature)
        {
            string filePath = ResolveFileLocationPath(fileName);
            FileStream strmConfig = File.OpenRead(filePath);

            if (verifySignature)
            {
                //TODO: Verify the digital signature
                throw new NotImplementedException("LoadConfigFromFile: verifySignature is not implemented yet.");
            }

            return strmConfig;
        }


        private int ReadInt32(byte[] buffer, int position, out int val)
        {
            val = (((buffer[position] | (buffer[position + 1] << 8))
                | (buffer[position + 2] << 0x10)) | (buffer[position + 3] << 0x18));
            return position + 4;
        }

        private void LoadBinaryConfig(string fileName, out Rijndael mRijndaelAlg, out int length)
        {
            byte[] buffer = File.ReadAllBytes(fileName);
            byte[] data = ProtectedData.Unprotect(buffer,
                    ApplicationID.ToByteArray(), DataProtectionScope.LocalMachine);

            int position = 0;
            int keyLength;
            int ivLength;

            position = ReadInt32(data, position, out keyLength);
            position = ReadInt32(data, position, out ivLength);
            position = ReadInt32(data, position, out length);

            byte[] key = new byte[keyLength];
            byte[] iv = new byte[ivLength];
            Buffer.BlockCopy(data, position, key, 0, keyLength);
            position += keyLength;
            Buffer.BlockCopy(data, position, iv, 0, ivLength);

            mRijndaelAlg = new RijndaelManaged();
            mRijndaelAlg.Key = key;
            mRijndaelAlg.IV = iv;
        }

        private Stream DecryptAndLoadConfigFromFile(string fileNameInternal)
        {
            string fileName = ResolveFileLocationPath(fileNameInternal);

            if (!File.Exists(fileName + ".bin"))
                throw new IOException("Cannot start. Config file is missing: " + fileName + ".bin");
            if (!File.Exists(fileName))
                throw new IOException("Cannot start. Config file is missing: " + fileName);

            Rijndael alg;
            int configFileLength;
            LoadBinaryConfig(fileName + ".bin", out alg, out configFileLength);

            Stream strmConfig = File.OpenRead(fileName);
            strmConfig = new CryptoStream(strmConfig, alg.CreateDecryptor(), CryptoStreamMode.Read);
            strmConfig = new GZipStream(strmConfig, CompressionMode.Decompress);
            strmConfig = new StreamCounter(strmConfig, StreamCounter.CounterDirection.Read, configFileLength, true);

            //Ok, the stream that we need to sent to the load config needs to be seekable, 
            //so we will copy our encrypted stream in to a MemoryStream
            return ConvertToMemoryStream(strmConfig);

        }

        private MemoryStream ConvertToMemoryStream(Stream strmConfig)
        {
            MemoryStream memStr = new MemoryStream();
            byte[] buffer = new byte[5000];
            int len = 0;
            while (strmConfig.CanRead)
            {
                len = strmConfig.Read(buffer, 0, 5000);
                memStr.Write(buffer, 0, len);
            }

            memStr.Position = 0;
            return memStr;
        }

        /// <summary>
        /// This method check the relevant location paths for a valid configuration file.
        /// </summary>
        /// <param name="fileName">The file to search for.</param>
        /// <returns>The full file path.</returns>
        private string ResolveFileLocationPath(string fileName)
        {
            FileInfo assemblyLocation = new FileInfo(this.GetType().Assembly.Location);

            string strMainPath = assemblyLocation.DirectoryName + @"\" + fileName;
            if (File.Exists(strMainPath)) return strMainPath;

            //			strMainPath = Assembly.GetExecutingAssembly().CodeBase;
            //			if (File.Exists(strMainPath)) return strMainPath;

            return null;

        }
        #endregion

    }
}
