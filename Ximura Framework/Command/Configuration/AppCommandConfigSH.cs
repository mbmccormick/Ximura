//#region Copyright
//// *******************************************************************************
//// Copyright (c) 2000-2009 Paul Stancer.
//// All rights reserved. This program and the accompanying materials
//// are made available under the terms of the Eclipse Public License v1.0
//// which accompanies this distribution, and is available at
//// http://www.eclipse.org/legal/epl-v10.html
////
//// Contributors:
////     Paul Stancer - initial implementation
//// *******************************************************************************
//#endregion
//#region using
//using System;
//using System.Collections;
//using System.Xml;
//using System.Configuration;

//using CH = Ximura.Helper.Common;
//using RH = Ximura.Helper.Reflection;
//#endregion // using
//namespace Ximura.Framework
//{
//    /// <summary>
//    /// Summary description for AppCommand config section handler.
//    /// </summary>
//    public class AppCommandConfigSH : IXimuraConfigSH
//    {
//        #region Declarations
//        private bool? mEnabled = null;
//        private JobPriority? mPriority = null;
//        /// <summary>
//        /// Hashtable to store Settings with name/value pairs
//        /// </summary>
//        protected Hashtable htSettings = new Hashtable();
//        /// <summary>
//        /// This is the hash table to store the extended settings objects.
//        /// </summary>
//        protected Hashtable htSettingsExtended = null;
//        /// <summary>
//        /// This is the root node passed to the settings object.
//        /// </summary>
//        protected XmlNode mSettings = null;

//        #endregion
//        #region Constructor
//        /// <summary>
//        /// The default constructor of AppCommandConfigSH. This constructor does nothing.
//        /// </summary>
//        public AppCommandConfigSH(){}
//        #endregion

//        #region Create
//        /// <summary>
//        /// Create Configuration
//        /// </summary>
//        /// <param name="parent"></param>
//        /// <param name="configContext"></param>
//        /// <param name="section">the xml section</param>
//        /// <returns>this object itself</returns>
//        public virtual object Create(object parent, object configContext, System.Xml.XmlNode section)
//        {
//            mSettings = section.CloneNode(true);

////			// Create Setting Collection
////			// get the settings node
////			XmlNode nodeSettings = section.SelectSingleNode("./settings");
////			if (nodeSettings != null)
////			{
////				// add settings to setting collection
////				this.htSettings = AddSettings(nodeSettings);
////			}

//            return this;
//        }
//        #endregion
//        #region GetSetting
//        /// <summary>
//        /// Get Setting of specific Type
//        /// </summary>
//        /// <param name="type">setting type</param>
//        /// <returns>setting value</returns>
//        public string GetSetting(string type)
//        {
//            return NodeAsString(@"./settings/@" + type);
//        }
//        #endregion // GetSetting
//        #region GetSettingTimer
//        /// <summary>
//        /// Get Setting of specific Type
//        /// </summary>
//        /// <param name="type">setting type</param>
//        /// <returns>setting value</returns>
//        public string GetSettingTimer(string type)
//        {
//            return NodeAsString(@"./timerpoll/@" + type);
//        }
//        #endregion // GetSetting
//        #region GetSettingAsBool
//        /// <summary>
//        /// Get Setting of specific Type
//        /// </summary>
//        /// <param name="type">setting type</param>
//        /// <returns>setting value</returns>
//        public bool GetSettingAsBool(string type)
//        {
//            return NodeAsBool(@"./settings/@" + type);
//        }
//        #endregion // GetSetting
//        #region GetSettingExtended
//        /// <summary>
//        /// This method returns the extended setting object or null if the type 
//        /// specified does not exist.
//        /// </summary>
//        /// <param name="type">The name of the type.</param>
//        /// <param name="subType">The name of the subtype.</param>
//        /// <returns>The settings object containing the specific settings or null if the object can not be found.</returns>
//        public object GetSettingExtended(string type, string subType)
//        {
//            IXimuraConfigSH objSettings = null;

//            if (mSettings == null)
//                return null;

//            try
//            {
//                string path = @"./settings/" + type + @"/add[@name=""" + subType + @"""]";
//                XmlNode nodeSettingsRoot = mSettings.SelectSingleNode(path);
//                XmlNode nodeSettings = mSettings.SelectSingleNode(@"./" + subType);

//                if (nodeSettingsRoot == null || nodeSettings == null)
//                    return null;

//                //Check whether we have any cached settings objects
//                if (htSettingsExtended == null)
//                    htSettingsExtended = new Hashtable();
//                else
//                    if (htSettingsExtended.ContainsKey(path))
//                        return htSettingsExtended[path];

//                //Nothing cached, so create the settings object.
//                string strType = nodeSettingsRoot.SelectSingleNode("@type").Value;

//                objSettings = RH.CreateObjectFromType(strType) as IXimuraConfigSH;

//                objSettings.Create(null,null,nodeSettings);
//                htSettingsExtended.Add(path,objSettings);

//            }
//            catch (Exception ex)
//            {
//                XimuraAppTrace.WriteLine(ex.Message,"Settings");
//                return null;
//            }

//            return objSettings;
//        }
//        #endregion // GetSettingExtended

//        #region Node helper methods
//        /// <summary>
//        /// This method returns an Xpath setting value as a string.
//        /// </summary>
//        /// <param name="xPath"></param>
//        /// <returns></returns>
//        protected string NodeAsString(string xPath)
//        {
//            if (mSettings == null)
//                return null;

//            XmlNode attr = mSettings.SelectSingleNode(xPath);
//            return attr == null ? null : attr.InnerText;
//        }
//        /// <summary>
//        /// This method returns an Xpath setting as true if it is equal to 'true'
//        /// </summary>
//        /// <param name="xPath"></param>
//        /// <returns></returns>
//        protected bool NodeAsBool(string xPath)
//        {
//            return NodeAsString(xPath) == "true";
//        }
//        #endregion // Node helper methods

//        #region AddSettings
//        /// <summary>
//        /// add all attributes of section as new name/value pair hashtable
//        /// </summary>
//        /// <param name="section">xml section of Settings</param>
//        /// <returns>Returns a new hashtable containing the settings.</returns>
//        protected Hashtable AddSettings(System.Xml.XmlNode section)
//        {
//            // permission collection
//            Hashtable htSettings = new Hashtable();
//            // add settings to setting collection
//            XmlAttributeCollection colAttrSetting = section.Attributes;
//            foreach(XmlAttribute currAttrSetting in colAttrSetting)
//            {
//                htSettings.Add(currAttrSetting.Name, currAttrSetting.Value);
//            }
//            return htSettings;
//        }
//        #endregion // AddSettings

//        #region Enabled
//        /// <summary>
//        /// This property determines whether the command is enabled.
//        /// </summary>
//        public bool Enabled
//        {
//            get
//            {
//                if (!mEnabled.HasValue)
//                    mEnabled = mSettings == null || !(NodeAsString("@enabled") == "false");
//                    return mEnabled.Value;
//            }
//        }
//        #endregion // Enabled
//        #region Priority
//        /// <summary>
//        /// This property determines whether the command is enabled.
//        /// </summary>
//        public JobPriority Priority
//        {
//            get
//            {
//                if (!mPriority.HasValue)
//                {
//                    try
//                    {
//                        string data = NodeAsString("@priority");

//                        if (data == null || data == "")
//                            mPriority = JobPriority.Normal;
//                        else
//                            mPriority = (JobPriority)Enum.Parse(typeof(JobPriority), data);
//                    }
//                    catch
//                    {
//                        mPriority = JobPriority.Normal;
//                    }
//                }

//                return mPriority.Value;
//            }
//        }
//        #endregion // Priority
//    }
//}
