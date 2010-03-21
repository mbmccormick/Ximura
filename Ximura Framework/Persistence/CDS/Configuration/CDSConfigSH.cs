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
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Configuration;

using Ximura.Server;
using Ximura.Command;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
using Ximura.Helper;
#endregion // using
namespace Ximura.Persistence
{
    ///// <summary>
    ///// The CDSConfigSH is used to provide the settings for the CDS object.
    ///// </summary>
    //public class CDSConfigSH : FSMConfigSH, ICDSConfigSH
    //{
    //    #region Declarations
    //    private XmlNode mSectionNode = null;
    //    /// <summary>
    //    /// This hashtable contains the mappings.
    //    /// </summary>
    //    Hashtable htMappings = new Hashtable();
    //    /// <summary>
    //    /// This hashtable contains the connections.
    //    /// </summary>
    //    Hashtable htConnections = new Hashtable();
    //    /// <summary>
    //    /// This hashtable contains the PM ConfigSHs.
    //    /// </summary>
    //    Hashtable htPMConfigSHs = new Hashtable();

    //    /// <summary>
    //    /// This is the PM config cache.
    //    /// </summary>
    //    Dictionary<string, ICDSPersistenceManagerConfigSH> pmCache;
    //    /// <summary>
    //    /// This is the sync collection.
    //    /// </summary>
    //    private object syncPM = new object();
    //    #endregion
    //    #region Constructors
    //    /// <summary>
    //    /// This is the default constructor for the CDS Config section handler.
    //    /// </summary>
    //    public CDSConfigSH()
    //    {
    //        pmCache = new Dictionary<string, ICDSPersistenceManagerConfigSH>();
    //    }

    //    #endregion
	
    //    #region IXimuraCDSConfigSH Members
    //    #region IConfigurationSectionHandler Members

    //    /// <summary>
    //    /// Create Config Object
    //    /// </summary>
    //    /// <param name="parent"></param>
    //    /// <param name="configContext"></param>
    //    /// <param name="section"></param>
    //    /// <returns></returns>
    //    public override object Create(object parent, object configContext, System.Xml.XmlNode section)
    //    {
    //        // Create Setting Collection
    //        // get the settings node
    //        base.Create(parent, configContext, section);

    //        // save the section node
    //        this.mSectionNode = section;

    //        // Create Mapping Collection
    //        // get all the mappings
    //        XmlNodeList nodelistMappings = section.SelectNodes("./mappings/add");
    //        // loop thru all mappings to get the mapping settings
    //        foreach(XmlNode currNodeMapping in nodelistMappings)
    //        {
    //            // get mapping setting from the current mapping
    //            Hashtable htSettings = base.AddSettings(currNodeMapping);
    //            // add the settings to the mapping collection
    //            this.htMappings.Add(currNodeMapping.Attributes.GetNamedItem("name").Value, htSettings);

    //        }

    //        string connectionCol = section.SelectSingleNode(@"./connections/@conntype").InnerText;
    //        // Create Connection Collection
    //        // get all the connections
    //        XmlNodeList nodelistConnections = section.SelectNodes(@"./connections/connection[@connid='" + connectionCol  + @"']/add");
    //        // loop thru all connections to get the connection settings
    //        foreach(XmlNode currNodeConnection in nodelistConnections)
    //        {
    //            // get connection setting from the current connection
    //            Hashtable htSettings = base.AddSettings(currNodeConnection);
    //            // add the settings to the connection collection
    //            this.htConnections.Add(currNodeConnection.Attributes.GetNamedItem("name").Value, htSettings);

    //        }

    //        // Create PM Collection
    //        // get all the PM ConfigSHs
    //        XmlNodeList nodelistPMConfigSHs = section.SelectNodes("./persistencemanagers/add");
    //        // loop thru all PM ConfigSHs to get the PM ConfigSH settings
    //        foreach(XmlNode currNodePMConfigSH in nodelistPMConfigSHs)
    //        {
    //            // get PM ConfigSH setting from the current PM ConfigSH
    //            Hashtable htSettings = base.AddSettings(currNodePMConfigSH);
    //            // add the settings to the PM ConfigSH collection
    //            this.htPMConfigSHs.Add(currNodePMConfigSH.Attributes.GetNamedItem("name").Value, htSettings);

    //        }

    //        return this;
    //    }

    //    #endregion

    //    /// <summary>
    //    /// Get Mapping Settings by Name with type/value pairs of settings
    //    /// </summary>
    //    /// <param name="Name">map name</param>
    //    /// <returns>hashtable of type/value pairs</returns>
    //    public Hashtable GetMapping(string Name)
    //    {
    //        return (Hashtable)this.htMappings[Name];
    //    }

    //    /// <summary>
    //    /// Get Mapping Setting of specific Map and Type
    //    /// </summary>
    //    /// <param name="Name">map name</param>
    //    /// <param name="Type">setting type</param>
    //    /// <returns>setting value</returns>
    //    public string GetMappingSetting(string Name, string Type)
    //    {
    //        Hashtable htMapping = GetMapping(Name);
    //        if (htMapping == null)
    //        {
    //            return null;
    //        }
    //        else
    //        {
    //            return (string)htMapping[Type];
    //        }
    //    }
		
    //    /// <summary>
    //    /// Get Connection Settings by Name with type/value pairs of settings
    //    /// </summary>
    //    /// <param name="Name">connection name</param>
    //    /// <returns>hashtable of type/value pairs</returns>
    //    public Hashtable GetConnection(string Name)
    //    {
    //        return (Hashtable)this.htConnections[Name];
    //    }

    //    /// <summary>
    //    /// Get Connection Setting of specific Connection and Type
    //    /// </summary>
    //    /// <param name="Name">connection name</param>
    //    /// <param name="Type">setting type</param>
    //    /// <returns>setting value</returns>
    //    public string GetConnectionSetting(string Name, string Type)
    //    {
    //        Hashtable htConnection = GetConnection(Name);
    //        if (htConnection == null)
    //        {
    //            return null;
    //        }
    //        else
    //        {
    //            return (string)htConnection[Type];
    //        }
    //    }

    //    /// <summary>
    //    /// Get All Connection Strings
    //    /// </summary>
    //    /// <returns>setting value</returns>
    //    public Dictionary<string,string> GetAllConnectionStrings()
    //    {
    //        Dictionary<string, string> htConnectionStrings = 
    //            new Dictionary<string, string>();

    //        foreach(Hashtable htConnection in this.htConnections.Values)
    //        {
    //            htConnectionStrings.Add((string)htConnection["name"], (string)htConnection["value"]);
    //        }
    //        return htConnectionStrings;
    //    }

    //    /// <summary>
    //    /// Get PMConfigSH Settings by Name with type/value pairs of settings
    //    /// </summary>
    //    /// <param name="Name">PMConfigSH name</param>
    //    /// <returns>hashtable of type/value pairs</returns>
    //    public Hashtable GetPMConfigSH(string Name)
    //    {
    //        return (Hashtable)this.htPMConfigSHs[Name];
    //    }

    //    /// <summary>
    //    /// Get PMConfigSH Setting of specific Name and Type
    //    /// </summary>
    //    /// <param name="Name">PMConfigSH name</param>
    //    /// <param name="Type">setting type</param>
    //    /// <returns>setting value</returns>
    //    public string GetPMConfigSHSetting(string Name, string Type)
    //    {
    //        Hashtable htPMConfigSH = GetPMConfigSH(Name);
    //        if (htPMConfigSH == null)
    //        {
    //            return null;
    //        }
    //        else
    //        {
    //            return (string)htPMConfigSH[Type];
    //        }
    //    }

    //    /// <summary>
    //    /// This method is used to retrieve the protocol settings
    //    /// </summary>
    //    /// <param name="PMSection">The protocol name</param>
    //    /// <returns>An object that implements IXimuraPersistenceManagerConfigSH and contains the
    //    /// persistence manager's settings.</returns>
    //    public ICDSPersistenceManagerConfigSH GetPersistenceManagerSettings(string PMSection)
    //    {
    //        // TODO:  Add CDSConfigSH.getPersistenceManagerSettings implementation
    //        if (pmCache.ContainsKey(PMSection))
    //            return pmCache[PMSection];

    //        lock (syncPM)
    //        {
    //            if (pmCache.ContainsKey(PMSection))
    //                return pmCache[PMSection];
    //            // get the mapping
    //            string PMMap = this.GetMappingSetting(PMSection, "value");
    //            if (PMMap == "" || PMMap == null)
    //            {
    //                // try to get the Default one
    //                PMMap = this.GetMappingSetting("{any}", "value");
    //            }
    //            if (PMMap == "" || PMMap == null)
    //                return null;

    //            XmlNode myNode = this.mSectionNode.SelectSingleNode(PMMap);
    //            if (myNode == null)
    //                return null;

    //            // get the handler type
    //            string strType = this.GetPMConfigSHSetting(PMMap, "type");

    //            // choose the handler and return the collection
    //            IConfigurationSectionHandler sectionHandler =
    //                RH.CreateObjectFromType(strType) as IConfigurationSectionHandler;

    //            if (sectionHandler == null)
    //            {
    //                int intIndex = strType.IndexOf(",");
    //                string strHandler = (intIndex < 0) ? strType : strType.Substring(0, intIndex);
    //                sectionHandler =
    //                    RH.CreateObjectFromType(strHandler) as IConfigurationSectionHandler;
    //            }

    //            pmCache.Add(PMSection, sectionHandler.Create(null, null, myNode) as ICDSPersistenceManagerConfigSH);
    //            return pmCache[PMSection];
    //        }
    //    }

    //    /// <summary>
    //    /// This method is used to retrieve the connection string for specific catalog
    //    /// </summary>
    //    /// <param name="ConectionMapping">Mapping Name</param>
    //    /// <returns>Connection string</returns>
    //    public string ResolveConnectionString(string ConnectionMapping)
    //    {
    //        return this.GetConnectionSetting(ConnectionMapping, "value");
    //    }
    //    #endregion



    //    #region IXimuraCDSConfigSH Members

    //    public bool SkipConnectivityTest
    //    {
    //        get 
    //        {
    //            return this.GetSettingAsBool("skipconnectivitytest");
    //        }
    //    }

    //    #endregion
    //}
}
