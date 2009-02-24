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
//using System.Xml.XPath;

//using Ximura;
//using Ximura.Server;
//using Ximura.Command;
//#endregion // using
//namespace Ximura.Logging
//{
//    public class LoggerConfigSH : AppCommandConfigSH, IXimuraLoggerConfigSH
//    {
//        private Hashtable htSwitches = new Hashtable();

//        public LoggerConfigSH(){}

//        public int GetSwitchValue(string Type)
//        {
//            if (Type == null)
//                return 0;

//            if (!htSwitches.ContainsKey(Type))
//                return 0;

//            return (int)this.htSwitches[Type];
//        }

	
//        public override object Create(object parent, object configContext, XmlNode section)
//        {
//            object toReturn = base.Create (parent, configContext, section);

//            ProcessSwitches(section);

//            return toReturn;
//        }

//        private void ProcessSwitches(XmlNode section)
//        {
//            XmlNodeList switches = section.SelectNodes(".//switches/switch");

//            string swName = null;
//            string swValue = null;

//            foreach(XmlNode configSwitch in switches)
//            {
//                try
//                {
//                    swName = configSwitch.Attributes["name"].InnerText;
//                    swValue = configSwitch.Attributes["value"].InnerText;
//                }
//                catch (Exception)
//                {
//                    swName = null;
//                }

//                if (swName!=null)
//                {
//                    if (swValue == null) swValue="0";
//                    if (!htSwitches.ContainsKey(swName))
//                        htSwitches.Add(swName, int.Parse(swValue));

//                    swName=null;
//                    swValue=null;
//                }
//            }
//        }

//        #region IXimuraLoggerSettings Members


//        public string LoggerID
//        {
//            get { return GetSetting("name"); }
//        }

//        public int LogLevel
//        {
//            get 
//            {
//                int value;
//                if (!int.TryParse(GetSetting("loglevel"), out value))
//                    return 0;

//                return value; 
//            }
//        }

//        #endregion

//        #region IXimuraLoggerSettings Members


//        public string LoggerName
//        {
//            get { return GetSetting("defaultCategory"); }
//        }

//        #endregion
//    }
//}
