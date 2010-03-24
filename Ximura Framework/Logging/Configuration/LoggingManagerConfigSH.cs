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
//using System.Collections.Generic;
//using System.Xml;
//using System.Xml.XPath;

//using Ximura;
//using Ximura.Framework;
//using Ximura.Framework;
//#endregion // using
//namespace Ximura.Logging
//{
//    /// <summary>
//    /// LoggingManagerConfigSH provides the configuration options for the Extensible logging provider..
//    /// </summary>
//    public class LoggingManagerConfigSH : AppCommandConfigSH, IXimuraLoggingManagerConfigSH
//    {
//        #region Declarations
//        private List<string> mProviders = null;
//        private XmlNode mSettingsNode = null;

//        #endregion

//        public override object Create(object parent, object configContext, 
//            System.Xml.XmlNode section)
//        {
//            base.Create(parent,configContext,section);

//            mSettingsNode=section;

//            InitiateProviderList(section);

//            return this;
//        }

//        private void InitiateProviderList(XmlNode section)
//        {
//            XmlNodeList list = section.SelectNodes(@".//trace/listeners/add/@name");

//            if (mProviders == null)
//                mProviders = new List<string>();
//            else
//                mProviders.Clear();

//            foreach(XmlNode node in list)
//                mProviders.Add(node.InnerText);
//        }

//        public IEnumerable<string> Loggers()
//        {
//            return mProviders;
//        }

//        public string LoggerType(string logger)
//        {
//            XmlNodeList list = 
//                mSettingsNode.SelectNodes(@".//trace/listeners/add[@name='" + logger + @"']/@type");

//            if (list == null || list.Count==0)
//                return null;

//            return list[0].InnerText;
//        }

//        public IXimuraLoggerConfigSH getLoggerSettings(string provider)
//        {
//            LoggerConfigSH newProvider = new LoggerConfigSH();

//            XmlNode providerSettingsNode = 
//                mSettingsNode.SelectSingleNode(@".//trace/listenersConfig/" + provider);

//            newProvider.Create((object)null,(object)null,providerSettingsNode);

//            return newProvider;
//        }
//    }
//}
