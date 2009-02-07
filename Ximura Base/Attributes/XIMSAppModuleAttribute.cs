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

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura
{
	/// <summary>
	/// This attribute is used to define command properties.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class,AllowMultiple=false)]
	public sealed class XimuraAppModuleAttribute : System.Attribute
	{
		#region Declaration

		private string mName, mDescription, mConfigClass;
		private Guid mCmdID = Guid.Empty;
        private bool mForceConfig = false;

		#endregion
		
		#region Class Constructors
		/// <summary>
		/// The XimuraAppCommandIDAttribute is used to uniquely identify a command within the Ximura system.
		/// </summary>
		/// <param name="ID">A string representation of a GUID, i.e. in the form 6364755B-97B9-4799-B8BC-3D98EB786C92</param>
		public XimuraAppModuleAttribute(string ID):
            this(ID, "", null, false, null){}
        /// <summary>
        /// The XimuraAppCommandIDAttribute is used to uniquely identify a command within the Ximura system.
        /// </summary>
        /// <param name="ID">A string representation of a GUID, i.e. in the form 
        /// 6364755B-97B9-4799-B8BC-3D98EB786C92</param>
        /// <param name="name">The friendly name of the command. 
		/// This will be used in the config file when parsing command set up paramters.</param>
		public XimuraAppModuleAttribute(string ID, string name):
            this(ID, name, null, false, null) { }
        /// <summary>
        /// The XimuraAppCommandIDAttribute is used to uniquely identify a command within the Ximura system.
        /// </summary>
        /// <param name="ID">A string representation of a GUID, i.e. in the form 
        /// 6364755B-97B9-4799-B8BC-3D98EB786C92</param>
        /// <param name="name">The friendly name of the command. 
        /// This will be used in the config file when parsing command set up paramters.</param>
        /// <param name="configClass">The config class for the command.</param>
        public XimuraAppModuleAttribute(string ID, string name, string configClass):
            this(ID, name, configClass, false, null) { }
        /// <summary>
        /// The XimuraAppCommandIDAttribute is used to uniquely identify a command within the Ximura system.
        /// </summary>
        /// <param name="ID">A string representation of a GUID, i.e. in the form 
        /// 6364755B-97B9-4799-B8BC-3D98EB786C92</param>
        /// <param name="name">The friendly name of the command. 
        /// This will be used in the config file when parsing command set up paramters.</param>
        /// <param name="configClass">The config class for the command.</param>
        /// <param name="description">The description for the command.</param>
        public XimuraAppModuleAttribute(string ID, string name, string configClass, 
            bool forceConfigClass, string description)
        {
            mCmdID = new Guid(ID);
            mName = name;
            mConfigClass = configClass;
            mConfigClass = configClass;
            mDescription = description;
            mForceConfig = forceConfigClass;
        }

		#endregion

		#region Property Fields

		/// <summary>
		/// The Guid of the module
		/// </summary>
		public Guid ID
		{
			get{return mCmdID;}
		}

		/// <summary>
		/// The name of the module. This is used to retrieve app settings.
		/// </summary>
		public string Name
		{
			get{return mName;}
		}

        /// <summary>
        /// This class returns the class type for the config settings class for the command.
        /// </summary>
        public string ConfigClass
        {
            get
            {
                return mConfigClass;
            }
        }

        /// <summary>
        /// This class returns the class type for the config settings class for the command.
        /// </summary>
        public bool ForceConfigClass
        {
            get
            {
                return mForceConfig;
            }
        }

        /// <summary>
        /// The friendly description of the module.
        /// </summary>
        public string Description
        {
			get { return mDescription; }
        }

		#endregion
	}
}