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
using System.Linq;
using System.Threading;
using System.Timers;
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
using System.Configuration.Install;
using System.Windows.Forms;

using Ximura;

using Ximura.Framework;
using RH = Ximura.Reflection;
#endregion // using
namespace Ximura.Windows
{
    /// <summary>
    /// The ApplicationServiceInstaller is responsible for providing the config file from the base stream.
    /// </summary>
    public class AppServerInstaller : System.Configuration.Install.Installer
    {
        #region Constructor / Destructor
        public AppServerInstaller():this(null)
        {
        }
        /// <summary>
        /// This is the default constructor for the service.
        /// </summary>
        public AppServerInstaller(AppServerAttribute attr)
        {
        }
        #endregion

        #region Install(IDictionary stateSaver)
        /// <summary>
        /// This override of the install method copies any config files to the install directory
        /// </summary>
        /// <param name="stateSaver">The state saver</param>
        public override void Install(IDictionary stateSaver)
        {
            // TODO:  Add ApplicationServiceInstaller.Install implementation
            base.Install(stateSaver);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show("Starting config file install..");
#endif
            ProcessAssemblies(stateSaver);
        }
        #endregion // Install(IDictionary stateSaver)
        #region Uninstall(IDictionary savedState)
        /// <summary>
        /// This method uninstalls the application.
        /// </summary>
        /// <param name="savedState"></param>
        public override void Uninstall(IDictionary savedState)
        {
#if (DEBUG)
            MessageBox.Show("Starting config file uninstall..");
#endif
            ConfigurationFilesRemove(savedState);
            EventLoggersRemove(savedState);
            base.Uninstall(savedState);
        }
        #endregion // Uninstall(IDictionary savedState)

        #region ProcessAssemblies(IDictionary savedState)
        /// <summary>
        /// This method processes the installer assemblies
        ///  the export the config file where specified.
        /// </summary>
        /// <param name="savedState">The saved state parameter.</param>
        private void ProcessAssemblies(IDictionary savedState)
        {
            GetType().Assembly.GetReferencedAssemblies()
                .Select(an => GetAssemblyFromName(an))
                .Where(a => a != null)
                .InsertAtStart(GetType().Assembly)
                .ForEach(a => ProcessAssembly(a, savedState));
        }
        #endregion // ProcessConfigFiles(IDictionary savedState)
        #region ProcessAssembly(Assembly theAssembly, IDictionary savedState)
        private void ProcessAssembly(Assembly theAssembly, IDictionary savedState)
        {
            theAssembly.GetTypes()
                .Where(t => t.IsClass)
                .ForEach(t => ProcessClassType(t, savedState));
        }
        #endregion
        #region ProcessClassType(Type AppServerType, IDictionary savedState)
        private void ProcessClassType(Type classType, IDictionary savedState)
        {
            ProcessEventLoggers(classType, savedState);

            ProcessAppServerConfiguration(classType, savedState);
        }
        #endregion // ProcessConfigFilesFromType(Type AppServerType, IDictionary savedState)

        #region ProcessAppServerConfiguration(Type classType, IDictionary savedState)
        private void ProcessAppServerConfiguration(Type classType, IDictionary savedState)
        {
            try
            {
                ArrayList configFiles = GetArrayList(savedState, "XimuraConfigFiles");

                //Process the installer attributes.
                classType.GetCustomAttributes(typeof(XimuraInstallerAppServerConfigAttribute), true)
                    .OfType<XimuraInstallerAppServerConfigAttribute>()
                    .ForEach(a => ProcessAppServerTypeConfiguration(a.AppServerType, configFiles));
            }
            catch (Exception ex1)
            {
                MessageBox.Show("AppServer installation error:" + Environment.NewLine + Environment.NewLine + ex1.Message);
            }
        }

        private void ProcessAppServerTypeConfiguration(Type AppServerType, ArrayList configFiles)
        {
            //Get the app server attribute.
            XimuraAppServerAttribute apSrvAttr = AppServerType
                .GetCustomAttributes(typeof(XimuraAppServerAttribute), true)
                .OfType<XimuraAppServerAttribute>()
                .FirstOrDefault();

            //Get the app server ID attribute.
            XimuraApplicationIDAttribute apIDAttr = AppServerType
                .GetCustomAttributes(typeof(XimuraApplicationIDAttribute), true)
                .OfType<XimuraApplicationIDAttribute>()
                .FirstOrDefault();

            //Get the app server attribute.
            XimuraAppServerConfigSystemAttribute apSrvConfigSysAttr = AppServerType
                .GetCustomAttributes(typeof(XimuraAppServerConfigSystemAttribute), true)
                .OfType<XimuraAppServerConfigSystemAttribute>()
                .FirstOrDefault();

            //Get the app server attribute.
            XimuraAppServerConfigCommandAttribute apSrvConfigComAttr = AppServerType
                .GetCustomAttributes(typeof(XimuraAppServerConfigCommandAttribute), true)
                .OfType<XimuraAppServerConfigCommandAttribute>()
                .FirstOrDefault();

            if (apSrvAttr != null && apIDAttr != null)
            {
                if (apSrvConfigSysAttr != null && !configFiles.Contains(apSrvConfigSysAttr.ConfigLocation))
                    ConfigurationSet(apSrvAttr, apIDAttr, AppServerType, configFiles, apSrvConfigSysAttr);

                if (apSrvConfigComAttr != null && !configFiles.Contains(apSrvConfigComAttr.ConfigLocation))
                    ConfigurationSet(apSrvAttr, apIDAttr, AppServerType, configFiles, apSrvConfigComAttr);
            }
        }
        #endregion // ProcessAppServerConfiguration(Type AppServerType, IDictionary savedState)

        #region ConfigurationSet
        private void ConfigurationSet(XimuraAppServerAttribute apSrvAttr, XimuraApplicationIDAttribute apIDAttr,
            Type AppServerType, ArrayList configFiles, XimuraAppServerConfigurationAttribute config)
        {
            try
            {
                if (apSrvAttr == null)
                    throw new ArgumentNullException("XimuraAppServerAttribute is null.");
                if (apIDAttr == null)
                    throw new ArgumentNullException("XimuraApplicationIDAttribute is null");

                Rijndael alg = new RijndaelManaged();

                using (Stream theStream =
                    AppServerType.Assembly.GetManifestResourceStream(config.ConfigLocation))
                {

                    if (theStream == null)
                        return;

                    ProcessConfigStream(theStream);

                    //OK, we need to save the stream to the relevant location
                    FileInfo assemblyFileInfo = new FileInfo(AppServerType.Assembly.Location);

                    string configFilePath = assemblyFileInfo.DirectoryName + @"\" + config.ConfigLocation;
                    string configFileConfirmPath = assemblyFileInfo.DirectoryName + @"\" + config.ConfigLocation + ".bin";

                    if (File.Exists(configFilePath))
                        File.Delete(configFilePath);

                    if (File.Exists(configFileConfirmPath))
                        File.Delete(configFileConfirmPath);

                    Stream outFile = null;
                    byte[] buffer = new byte[3000];

                    try
                    {
                        outFile = GetOutputStream(alg, config.ConfigOptions, configFilePath);

                        int bytesLength = 0;
                        int bytesRead = 0;
                        while (theStream.CanRead)
                        {
                            bytesRead = theStream.Read(buffer, 0, 3000);
                            bytesLength += bytesRead;
                            if (bytesRead == 0) break;

                            outFile.Write(buffer, 0, bytesRead);
                        }
                        //Add the file to the collection so that it will get deleted when the application
                        //is uninstalled.
                        configFiles.Add(configFilePath);
                        configFiles.Add(configFileConfirmPath);

                        ConfigurationWriteCompletionBinary(alg, config.ConfigOptions, bytesLength,
                            configFileConfirmPath, theStream, apIDAttr.ApplicationID);

                    }
                    finally
                    {
                        if (outFile != null)
                        {
                            outFile.Flush();
                            outFile.Close();
                        }
                    }

                    theStream.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Install:" + ex.Message + "\r\n" + ex.ToString());
                throw ex;
            }

        }
        #endregion // SetConfigFile
        #region ConfigurationWriteCompletionBinary
        protected virtual void ConfigurationWriteCompletionBinary(Rijndael alg, AppServerConfigOptions options, int size,
            string configFileConfirmPath, Stream outgoing, Guid ApplicationID)
        {
            byte[] data = null;

            switch (options)
            {
                case AppServerConfigOptions.FileDigitallySigned:
                    data = GenerateHash(outgoing);
                    break;
                case AppServerConfigOptions.FileEncrypted:
                    byte[] buffer = GenerateCryptoBlob(alg, size);
                    if (buffer != null)
                    {
                        data = ProtectedData.Protect(buffer, ApplicationID.ToByteArray(), DataProtectionScope.LocalMachine);
                    }
                    break;
            }

            if (data != null)
                File.WriteAllBytes(configFileConfirmPath, data);
        }
        #endregion // WriteCompletionBinary
        #region ConfigurationFilesRemove(IDictionary savedState)
        private void ConfigurationFilesRemove(IDictionary savedState)
        {
            if (!savedState.Contains("XimuraConfigFiles"))
                return;

            ArrayList configFiles = savedState["XimuraConfigFiles"] as ArrayList;
            if (configFiles == null)
                return;

            try
            {
                configFiles.Cast<string>()
                    .Where(f => File.Exists(f))
                    .ForEach(f => File.Delete(f));
            }
            catch { }

        }
        #endregion // RemoveConfigFiles(IDictionary savedState)

        #region ProcessEventLoggers(Type AppServerType, IDictionary savedState)
        private void ProcessEventLoggers(Type AppServerType, IDictionary savedState)
        {
            //Check whether the saved state key exists
            ArrayList loggers = GetArrayList(savedState, "XimuraLoggers");

            try
            {
                //Process the installer attributes.
                AppServerType.GetCustomAttributes(typeof(XimuraInstallerEventLoggerAttribute), true)
                    .OfType<XimuraInstallerEventLoggerAttribute>()
                    .Where(a => { return a.LoggerType.IsAssignableFrom(typeof(EventLogLogger)); })
                    .ForEach(a => EventLoggerAdd(a, loggers));
            }
            catch (Exception ex1)
            {
                MessageBox.Show("Logger installation error:" + Environment.NewLine + Environment.NewLine + ex1.Message);
            }
        }
        #endregion // ProcessEventLoggers(Type AppServerType, IDictionary savedState)
        #region EventLoggerAdd(XimuraAppServerLoggerAttribute logger, ArrayList loggersAdded)
        private void EventLoggerAdd(XimuraInstallerEventLoggerAttribute logger, ArrayList loggersAdded)
        {
#if (DEBUG)
            MessageBox.Show(string.Format(@"{0}/{1} - {2}",
                logger.LoggerID, logger.LoggerName, logger.LoggerType.ToString()));
#endif
            bool exists = EventLog.SourceExists(logger.LoggerID);

            if (!exists)
            {
                loggersAdded.Add(logger.LoggerID);
                EventLog.CreateEventSource(logger.LoggerID, logger.LoggerName);
            }
        }
        #endregion // EventLoggerAdd(XimuraAppServerLoggerAttribute logger, ArrayList loggersAdded)
        #region EventLoggersRemove(IDictionary savedState)
        /// <summary>
        /// This method removes any installed loggers.
        /// </summary>
        /// <param name="savedState">The saved state that contains the log install history.</param>
        private void EventLoggersRemove(IDictionary savedState)
        {
            if (!savedState.Contains("XimuraLoggers"))
                return;

            ArrayList loggers = savedState["XimuraLoggers"] as ArrayList;
            if (loggers == null || loggers.Count == 0)
                return;

            DialogResult result = MessageBox.Show(
                "Do you want to delete the event logs that were installed for this application?",
                "Event log deletion", MessageBoxButtons.YesNoCancel);

            if (result != DialogResult.Yes)
                return;

            loggers
                .Cast<string>()
                .ForEach(s => EventLoggerRemove(s));
        }
        #endregion // RemoveLoggers(IDictionary savedState)
        #region EventLoggerRemove(string logName)
        /// <summary>
        /// This method removes the specified logger.
        /// </summary>
        /// <param name="logName">The log name to remove.</param>
        private void EventLoggerRemove(string logSource)
        {
            if (!EventLog.SourceExists(logSource))
                return;

            string logName = EventLog.LogNameFromSourceName(logSource, ".");

            try
            {
                EventLog.DeleteEventSource(logName);
            }
            catch (Exception ex)
            {
#if (DEBUG)
                MessageBox.Show(ex.Message);
#endif
            }

            try
            {
                EventLog.Delete(logName);
            }
            catch (Exception ex)
            {
#if (DEBUG)
                MessageBox.Show(ex.Message);
#endif
            }
        }
        #endregion // RemoveLogger(string logName)

        #region Helper functions
        #region GetArrayList(IDictionary savedState, string key)
        /// <summary>
        /// This method retrieves the array list.
        /// </summary>
        /// <param name="savedState">The persisted state.</param>
        /// <param name="key">The collection key.</param>
        /// <returns></returns>
        private ArrayList GetArrayList(IDictionary savedState, string key)
        {
            //Check whether the saved state key exists
            ArrayList configFiles = null;
            if (!savedState.Contains(key))
            {
                configFiles = new ArrayList();
                savedState.Add(key, configFiles);
            }
            else
            {
                configFiles = savedState[key] as ArrayList;
            }

            return configFiles;
        }
        #endregion // GetArrayList(IDictionary savedState, string key)

        private Assembly GetAssemblyFromName(AssemblyName theAssemblyName)
        {
            Assembly[] theAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly toReturn = null;
            foreach (Assembly loopAssembly in theAssemblies)
            {
                if (theAssemblyName.FullName == loopAssembly.FullName)
                {
                    toReturn = loopAssembly;
                    break;
                }
            }

            if (toReturn == null)
            {
                toReturn = AppDomain.CurrentDomain.Load(theAssemblyName);
            }

            return toReturn;
        }

        protected virtual Stream GetOutputStream(Rijndael alg, AppServerConfigOptions options, string configFilePath)
        {
            Stream outStream = new FileStream(configFilePath, FileMode.CreateNew);

            switch (options)
            {
                case AppServerConfigOptions.FileEncrypted:
                    outStream = new CryptoStream(outStream, alg.CreateEncryptor(), CryptoStreamMode.Write);
                    outStream = new GZipStream(outStream, CompressionMode.Compress);
                    break;
            }

            return outStream;
        }

        protected virtual byte[] GenerateCryptoBlob(Rijndael alg, int size)
        {
            byte[] blob = new byte[12 + alg.Key.Length + alg.IV.Length];
            int position = 0;
            position = WriteInt32(alg.Key.Length, blob, 0);
            position = WriteInt32(alg.IV.Length, blob, position);
            position = WriteInt32(size, blob, position);
            Buffer.BlockCopy(alg.Key, 0, blob, position, alg.Key.Length);
            position += alg.Key.Length;
            Buffer.BlockCopy(alg.IV, 0, blob, position, alg.IV.Length);
            return blob;
        }

        int WriteInt32(int value, byte[] buffer, int position)
        {
            buffer[position] = (byte)value;
            buffer[position + 1] = (byte)(value >> 8);
            buffer[position + 2] = (byte)(value >> 0x10);
            buffer[position + 3] = (byte)(value >> 0x18);
            return position + 4;
        }

        protected virtual byte[] GenerateHash(Stream outStream)
        {
            outStream.Position = 0;
            byte[] buffer = new byte[outStream.Length];
            outStream.Read(buffer, 0, (int)outStream.Length);

            SHA1 sha = new SHA1CryptoServiceProvider();

            return sha.ComputeHash(buffer);
        }

        protected virtual void ProcessConfigStream(Stream configStream)
        {
        }

        #endregion
    }
}