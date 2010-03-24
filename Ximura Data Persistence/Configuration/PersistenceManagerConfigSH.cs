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
using System.Xml;

using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{
    ///// <summary>
    ///// ProtocolServerConfigSH contains the extended configuration for the protocol command.
    ///// </summary>
    //public class PersistenceManagerConfigSH :
    //    AppCommandProcessConfigSH, ICDSPersistenceManagerConfigSH
    //{
    //    #region Declarations
    //    XmlNode commandConfig = null;
    //    /// <summary>
    //    /// This hashtable contains the access rights.
    //    /// </summary>
    //    Hashtable htAccessRights = new Hashtable();
    //    /// <summary>
    //    /// This hashtable contains the connection mappings.
    //    /// </summary>
    //    Hashtable htConnectionMappings = new Hashtable();

    //    #endregion
    //    #region Constructors
    //    /// <summary>
    //    /// The default constructor for a config setting handler.
    //    /// </summary>
    //    public PersistenceManagerConfigSH() { }
    //    #endregion

    //    #region Create
    //    /// <summary>
    //    /// The overriden Create object
    //    /// </summary>
    //    /// <param name="parent"></param>
    //    /// <param name="configContext"></param>
    //    /// <param name="section">The configuration node.</param>
    //    /// <returns></returns>
    //    public override object Create(object parent, object configContext, XmlNode section)
    //    {
    //        base.Create(parent, configContext, section);

    //        //We are only interested in the Protocol sections.
    //        commandConfig = section;

    //        // Create AccessRight Collection
    //        // get all the AccessRights
    //        XmlNodeList nodelistAccessRights = section.SelectNodes("./settings/accessrights/add");
    //        // loop thru all AccessRights to get the AccessRight settings
    //        foreach (XmlNode currNodeAccessRight in nodelistAccessRights)
    //        {
    //            // get AccessRight setting from the current AccessRight
    //            Hashtable htSettings = base.AddSettings(currNodeAccessRight);
    //            // add the settings to the AccessRight collection
    //            this.htAccessRights.Add(currNodeAccessRight.Attributes.GetNamedItem("name").Value, htSettings);

    //        }

    //        // Create ConnectionMapping Collection
    //        // get all the ConnectionMappings
    //        XmlNodeList nodelistConnectionMappings = section.SelectNodes("./settings/connectionmappings/add");
    //        // loop thru all connectionmappings to get the connectionmapping settings
    //        foreach (XmlNode currNodeConnectionMapping in nodelistConnectionMappings)
    //        {
    //            // get ConnectionMapping setting from the current ConnectionMapping
    //            Hashtable htSettings = base.AddSettings(currNodeConnectionMapping);
    //            // add the settings to the ConnectionMapping collection
    //            this.htConnectionMappings.Add(currNodeConnectionMapping.Attributes.GetNamedItem("name").Value, htSettings);

    //        }

    //        return this;
    //    }
    //    #endregion // Create

    //    #region GetAccessRight/GetAccessRightSetting
    //    /// <summary>
    //    /// Get AccessRight Settings by Name with type/value pairs of settings
    //    /// </summary>
    //    /// <param name="Name">catalog name</param>
    //    /// <returns>hashtable of type/value pairs</returns>
    //    public Hashtable GetAccessRight(string Name)
    //    {
    //        return (Hashtable)this.htAccessRights[Name];
    //    }

    //    /// <summary>
    //    /// Get AccessRight Setting of specific Map and Type
    //    /// </summary>
    //    /// <param name="Name">catalog name</param>
    //    /// <param name="Type">setting type</param>
    //    /// <returns>setting value</returns>
    //    public string GetAccessRightSetting(string Name, string Type)
    //    {
    //        Hashtable htAccessRight = GetAccessRight(Name);
    //        if (htAccessRight == null)
    //        {
    //            return null;
    //        }
    //        else
    //        {
    //            return (string)htAccessRight[Type];
    //        }
    //    }
    //    #endregion // GetAccessRight

    //    #region GetConnectionMapping/GetConnectionMappingSetting
    //    /// <summary>
    //    /// Get ConnectionMapping Settings by Name with type/value pairs of settings
    //    /// </summary>
    //    /// <param name="Name">catalog name</param>
    //    /// <returns>hashtable of type/value pairs</returns>
    //    public Hashtable GetConnectionMapping(string Name)
    //    {
    //        return (Hashtable)this.htConnectionMappings[Name];
    //    }

    //    /// <summary>
    //    /// Get ConnectionMapping Setting of specific Map and Type
    //    /// </summary>
    //    /// <param name="Name">catalog name</param>
    //    /// <param name="Type">setting type</param>
    //    /// <returns>setting value</returns>
    //    public string GetConnectionMappingSetting(string Name, string Type)
    //    {
    //        Hashtable htConnectionMapping = GetConnectionMapping(Name);
    //        if (htConnectionMapping == null)
    //        {
    //            return null;
    //        }
    //        else
    //        {
    //            return (string)htConnectionMapping[Type];
    //        }
    //    }
    //    #endregion // GetConnectionMapping

    //    #region getProtocolSettings
    //    /// <summary>
    //    /// This method retrieves the protocol settings from the command.
    //    /// </summary>
    //    /// <param name="Protocol">The protocol name</param>
    //    /// <returns>An object that implements the IXimuraProtocolConfigSH interface that contains the
    //    /// protocol settings, or null if the settings cannot be found.</returns>
    //    public ICDSPersistenceManagerConfigSH getProtocolSettings(string Protocol)
    //    {
    //        if (Protocol == null || Protocol == "")
    //            return null;

    //        PersistenceManagerConfigSH pmConfig = new PersistenceManagerConfigSH();

    //        //			XmlNode protocolNode = commandConfig.SelectSingleNode(@"Protocols/" + Protocol);
    //        //			if (protocolNode == null)
    //        //				return null;
    //        //
    //        //			protocolConfig.Create(null,null,protocolNode.CloneNode(true));

    //        return pmConfig as ICDSPersistenceManagerConfigSH;
    //    }
    //    #endregion // getProtocolSettings

    //    #region AccessRight
    //    /// <summary>
    //    /// Access Right
    //    /// </summary>
    //    /// <param name="Type">access type</param>
    //    /// <returns>access right</returns>
    //    public bool AccessRight(string Type)
    //    {
    //        // get the mapping
    //        string right = this.GetAccessRightSetting(Type, "value");
    //        if (right == "" || right == null)
    //        {
    //            // try to get the Default one
    //            XmlNode attr = commandConfig.SelectSingleNode(@"./accessrights/@default");

    //            if (attr == null)
    //                return false;

    //            right = attr.Value;
    //        }
    //        if (right == "" || right == null)
    //            return false;

    //        return Convert.ToBoolean(right);
    //    }
    //    #endregion

    //    #region ResolveConnectionMapping
    //    /// <summary>
    //    /// Resolve Connection Mapping for specific Catalog
    //    /// </summary>
    //    /// <param name="Catalog">catalog Name</param>
    //    /// <returns>connection mapping string</returns>
    //    public string ResolveConnectionMapping(string Catalog)
    //    {
    //        // get the mapping
    //        string mapping = this.GetConnectionMappingSetting(Catalog, "value");
    //        if (mapping == "" || mapping == null)
    //        {
    //            // try to get the Default one
    //            mapping = this.GetConnectionMappingSetting("{any}", "value");
    //        }
    //        if (mapping == "" || mapping == null)
    //            return null;

    //        return mapping;
    //    }
    //    #endregion

    //}
}
