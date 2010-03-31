#region using
using System;
using System.Data;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using Ximura.Communication;
using CH = Ximura.Common;
using XH = Ximura.XMLHelper;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This internal structure holds the cached mapping information.
    /// </summary>
    public class Mapping : MappingBase
    {
        #region Constructor
        public Mapping(XmlNode nodeMapping, XmlNamespaceManager NSM)
        {
            try
            {
                XmlNodeList nodeMatches = nodeMapping.SelectNodes("./r:match", NSM);
                XmlNode nodeTemplate = nodeMapping.SelectSingleNode("./r:template", NSM);
                XmlNode nodeProtocol = nodeMapping.SelectSingleNode("./r:protocol", NSM);
                XmlNode nodeResolver = nodeMapping.SelectSingleNode("./r:resolver", NSM);
                XmlNodeList nodeVariables = nodeMapping.SelectNodes("./r:variable", NSM);
                XmlNodeList nodeOutputs = nodeMapping.SelectNodes("./r:output", NSM);
                XmlNode nodeAuth = nodeMapping.SelectSingleNode("./r:auth", NSM);
                XmlNode nodeRedirect = nodeMapping.SelectSingleNode("./r:redirect", NSM);

                //mapping
                MappingID = nodeMapping.Attributes["id"].Value;

                MappingState = nodeMapping.Attributes["state"]!=null?nodeMapping.Attributes["state"].Value:null;
                MappingVerb = nodeMapping.Attributes["verb"].Value;

                MatchColl = new List<MatchHolder>();
                foreach (XmlNode nodeMatch in nodeMatches)
                {
                    MatchColl.Add(new MatchHolder(nodeMatch));
                }

                ValidFrom = nodeMapping.Attributes["validfrom"] != null ? DateConvert(nodeMapping.Attributes["validfrom"].Value) : null;
                ValidUpTo = nodeMapping.Attributes["validupto"] != null ? DateConvert(nodeMapping.Attributes["validupto"].Value) : null;

                //protocol
                if (nodeProtocol != null)
                    ProtocolState = nodeProtocol.InnerText;
                //resolver
                if (nodeResolver != null)
                    ResolverState = nodeResolver.InnerText;

                //redirect
                if (nodeRedirect != null)
                    Redirect = nodeRedirect.InnerText;

                //template
                if (nodeTemplate != null)
                    Template = nodeTemplate.InnerText;
                else
                    Template = null;

                VariableColl = new List<VariableHolder>();
                foreach (XmlNode nodeVariable in nodeVariables)
                    VariableColl.Add(new VariableHolder(nodeVariable));

                OutputColl = new List<OutputHolder>();
                foreach (XmlNode nodeOutput in nodeOutputs)
                    OutputColl.Add(new OutputHolder(nodeOutput));

                //auth
                if (nodeAuth != null)
                {
                    if (nodeAuth.Attributes["state"] != null)
                        AuthState = nodeAuth.Attributes["state"].Value;
                    else
                        AuthState = null;

                    AuthDomain = nodeAuth.Attributes["domain"].Value;
                }
                else
                {
                    AuthState = null;
                    AuthDomain = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion // Constructor

        private DateTime? DateConvert(string convert)
        {
            if (convert == null || convert == "")
                return null;

            DateTime data;
            if (DateTime.TryParse(convert, out data))
                return data;

            return null;
        }


        #region RXMatch(string data, out Match match)
        /// <summary>
        /// This is the match collection.
        /// </summary>
        /// <param name="data">The data to match.</param>
        /// <param name="match">The match paremeters.</param>
        /// <returns>Returns true if there is a match to the collection.</returns>
        public bool RXMatch(string data, out Match match, out Regex rx)
        {
            match = null;
            rx = null;

            if (ValidFrom.HasValue && DateTime.Now < ValidFrom.Value )
                return false;

            if (ValidUpTo.HasValue && ValidUpTo.Value <= DateTime.Now)
                return false;

            foreach (MatchHolder holder in MatchColl)
            {
                Match isOK = holder.RX.Match(data);
                if (isOK.Success)
                {
                    rx = holder.RX;
                    match = isOK;
                    return true;
                }
            }

            return false;
        }
        #endregion // RXMatch(string data, out Match match)
    }

    public class MappingBase : PoolableReturnableObjectBase
    {
        #region Declarations
        protected int? mPriority;
        #endregion // Declarations
        #region MappingBase()
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public MappingBase()
        {

        }
        #endregion // MappingSettings()

        #region Reset()
        /// <summary>
        /// This is the reset method for the object.
        /// </summary>
        public override void Reset()
        {
            ValidFrom = null;
            ValidUpTo = null;

            mPriority = 0;

            MappingID = null;

            ProtocolState = null;
            ResolverState = null;
            MappingState = null;

            MappingVerb = null;

            Template = null;

            Redirect = null;

            OutputColl = null;
            MatchColl = null;
            VariableColl = null;

            AuthState = null;
            AuthDomain = null;

            base.Reset();
        }
        #endregion // Reset()

        #region Public properties
        public string MappingID;
        public string MappingState;
        public string MappingVerb;

        public string ProtocolState;
        public string ResolverState;

        public string Template;

        public string Redirect;

        public List<VariableHolder> VariableColl;
        public List<OutputHolder> OutputColl;
        public List<MatchHolder> MatchColl;

        public string AuthState;
        public string AuthDomain;

        public DateTime? ValidFrom;
        public DateTime? ValidUpTo;
        #endregion // Public properties

        #region Priority
        /// <summary>
        /// The mapping priority.
        /// </summary>
        public virtual int Priority
        {
            get
            {
                if (mPriority.HasValue)
                    return mPriority.Value;
                return 0;
            }
        }
        #endregion // Priority
    }

    public class MappingSettings : MappingBase
    {
        #region MappingSettings()
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public MappingSettings()
        {

        }
        #endregion // MappingSettings()


        #region Set(Mapping map)
        /// <summary>
        /// This method sets the particular settings from the base class.
        /// </summary>
        /// <param name="map">The map to set.</param>
        public void Set(Mapping map)
        {
            SetParam(ref MappingID, map.MappingID);
            SetParam(ref ProtocolState, map.ProtocolState);
            SetParam(ref ResolverState, map.ResolverState);
            SetParam(ref MappingState, map.MappingState);
            SetParam(ref MappingVerb, map.MappingVerb);
            SetParam(ref Template, map.Template);

            SetParamList<OutputHolder>(ref OutputColl, map.OutputColl);
            SetParamList<VariableHolder>(ref VariableColl, map.VariableColl);

            //SetParam<List<MatchHolder>>(ref MatchColl, map.MatchColl);
            SetParam(ref AuthState, map.AuthState);
            SetParam(ref AuthDomain, map.AuthDomain);

            //Redirect should be nullable after the initial redirect.
            Redirect = map.Redirect;

        }
        #endregion // Set(Mapping map)


        private void SetParam(ref string value, string data)
        {
            SetParam<string>(ref value, data);
        }

        private void SetParam<P>(ref P value, P data) where P:class
        {
            if (data == null)
                return;

            value = data;
        }

        private void SetParamList<P>(ref List<P> value, List<P> data)
        {
            if (data == null)
                return;

            value = new List<P>(data);
            //value = data;
        }
    }

    #region VariableHolder Class
    /// <summary>
    /// This structure holds the output settings.
    /// </summary>
    public struct VariableHolder
    {
        public string VariableID;
        public string VariableType;
        public string Variable;

        public VariableHolder(XmlNode nodeVariable)
        {
            //output
            if (nodeVariable != null)
            {
                VariableID = nodeVariable.Attributes["id"].Value;
                VariableType = nodeVariable.Attributes["type"].Value;
                Variable = nodeVariable.InnerText;
            }
            else
            {
                VariableID = null;
                VariableType = null;
                Variable = null;
            }
        }
    }
    #endregion // Output
    #region OutputHolder Class
    /// <summary>
    /// This structure holds the output settings.
    /// </summary>
    public struct OutputHolder
    {
        public string OutputMIMEType;
        public string OutputType;
        public string Output;

        public OutputHolder(XmlNode nodeOutput)
        {
            //output
            if (nodeOutput != null)
            {
                if (nodeOutput.Attributes["mimetype"] != null)
                    OutputMIMEType = nodeOutput.Attributes["mimetype"].Value;
                else
                    OutputMIMEType = null;

                OutputType = nodeOutput.Attributes["type"].Value;
                Output = nodeOutput.InnerText;
            }
            else
            {
                OutputMIMEType = null;
                OutputType = null;
                Output = null;
            }
        }
    }
    #endregion // Output
    #region MatchHolder Class
    /// <summary>
    /// This is the Match class
    /// </summary>
    public struct MatchHolder
    {
        public string MatchType;
        private string strRegEx;
        public Regex RX;

        public MatchHolder(XmlNode nodeMatch)
        {
            //match
            MatchType = null;// nodeMatch.Attributes["type"].Value;
            strRegEx = nodeMatch.InnerText;
            RX = new Regex(strRegEx,
                RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
    }
    #endregion // Match

}
