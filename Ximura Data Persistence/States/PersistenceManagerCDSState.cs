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
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using RH = Ximura.Reflection;
using AH = Ximura.AttributeHelper;
using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This is the base state for persistence managers.
    /// </summary>
    //[ToolboxBitmap(typeof(XimuraResourcePlaceholder), "Ximura.Resources.PersistenceManager.bmp")]
    public class PersistenceManagerCDSState<CONT,DCONT,CONF> : CDSState<CONF, CDSPMPerformance>
        where CONT : DCONT
        where DCONT : Content
        where CONF : CommandConfiguration, new()
    {
        #region Declarations
        private object syncActionPermit = new object();
        /// <summary>
        /// This is the specific pool for the data content.
        /// </summary>
        protected IXimuraPool<CONT> mPool;

        private CDSStateActionPermitAttribute[] attrsCDSStateActionPermit;
        private CDSStateTypePermitAttribute attrCDSStateTypePermit;
        private Dictionary<CDSStateAction, bool> mActionPermitCache;
        private Dictionary<Type, bool> mTypePermitCache;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public PersistenceManagerCDSState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public PersistenceManagerCDSState(IContainer container)
            : base(container)
        {
            SetAttributes();

            mActionPermitCache = new Dictionary<CDSStateAction, bool>();
            mTypePermitCache = new Dictionary<Type, bool>();
        }
        #endregion // Constructors

        #region SetAttributes()
        /// <summary>
        /// This method is used to set the attributes for the state. This method is called during the 
        /// base constructor. You can override this method to add additional attributes.
        /// </summary>
        protected virtual void SetAttributes()
        {
            attrsCDSStateActionPermit = AH.GetAttributes<CDSStateActionPermitAttribute>(GetType());

            attrCDSStateTypePermit = AH.GetAttribute<CDSStateTypePermitAttribute>(GetType());
        }
        #endregion // SetAttributes()

        #region SupportsEntityAction(CDSStateAction action, Type objectType)
        /// <summary>
        /// This method should return true when the action and entity are supported. This method is used by the CDS to build
        /// the Execution plan for specific Entity types and actions.
        /// </summary>
        /// <returns>Returns -1 is the action is not supported, otherwise the combined order is returned.</returns>
        public override int SupportsEntityAction(CDSStateAction action, Type objectType)
        {
            if (!PermitType(objectType))
                return -1;

            if (!PermitAction(action))
                return -1;

            return PriorityCombined;
        }
        #endregion // SupportsEntityAction(CDSStateAction action, Type objectType)

        #region PermitAction(CDSStateAction action)
        /// <summary>
        /// This method scans the CDSStateActionPermit attributes and pull out the permit settings.
        /// </summary>
        /// <param name="action">The action to scan for.</param>
        protected virtual bool PermitAction(CDSStateAction action)
        {
            if (mActionPermitCache.ContainsKey(action))
                return mActionPermitCache[action];

            lock (syncActionPermit)
            {
                if (mActionPermitCache.ContainsKey(action))
                    return mActionPermitCache[action];

                bool? permit = null;
                int level = -1;

                foreach (CDSStateActionPermitAttribute attr in attrsCDSStateActionPermit)
                {
                    //If it's not for the action we want, skip it.
                    if (attr.Action != action)
                        continue;
                    //If we're above the level of this attribute, then skip.
                    if (attr.Priority < level)
                        continue;

                    //No value is set, so set it and continue.
                    if (!permit.HasValue)
                    {
                        permit = !attr.Deny;
                        level = attr.Priority;
                        continue;
                    }

                    //Deny is already set, and the level is the same, so continue.
                    if (!permit.Value && attr.Priority == level)
                        continue;

                    //This is a higher priority, or is a deny setting
                    permit = !attr.Deny;
                    level = attr.Priority;
                }

                mActionPermitCache.Add(action, permit.HasValue ? permit.Value : false);
            }

            return mActionPermitCache[action];
        }
        #endregion // PermitAction(CDSStateAction action)

        #region PermitType(Type objectType)
        /// <summary>
        /// This method determines whether the type specified is permitted for this
        /// </summary>
        /// <param name="objectType"></param>
        protected virtual bool PermitType(Type objectType)
        {
            if (objectType == typeof(DCONT))
                return true;

            if (objectType.IsSubclassOf(typeof(DCONT)))
                return true;

            return false;
        }
        #endregion // PermitType(Type objectType)

    }
}
