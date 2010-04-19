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
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;
using System.Xml;

using Ximura;
using Ximura.Data;

using Ximura.Data;
using RH = Ximura.Reflection;
using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This state loads the script and compiles it to produce the ModelData object.
    /// </summary>
    public class ModelDataTemplateCompilerCCState : ContentCompilerState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ModelDataTemplateCompilerCCState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public ModelDataTemplateCompilerCCState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        #region ModelTemplateLoad(ContentCompilerContext context)
        /// <summary>
        /// This method loads and validates the script.
        /// </summary>
        /// <param name="context">The current context.</param>
        public override void ModelTemplateLoad(ContentCompilerContext context)
        {
            string status=CH.HTTPCodes.OK_200;
            string templateRef = context.Request.Settings.Template;

            ModelTemplate template = context.TemplateGet("name", templateRef);
            if (template != null)
                status = CH.HTTPCodes.OK_200;

            if (status != CH.HTTPCodes.OK_200)
                throw new InvalidTemplateCCException(templateRef + " could not be found.");

            context.Template = template;
        }
        #endregion

        #region ModelTemplateCompile(ContentCompilerContext context)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">The current context.</param>
        public override void ModelTemplateCompile(ContentCompilerContext context)
        {
            byte[] blob = context.Template.Transform(context.Request.Data);

            Model contentModel = context.ContextSettings.PoolManager.GetPoolManager<Model>().Get();
            contentModel.Load(blob, 0, blob.Length);

            contentModel.IDContent = Guid.NewGuid();
            contentModel.IDVersion = Guid.NewGuid();

            context.ModelData = contentModel;
        }
        #endregion // ModelTemplateCompile(ContentCompilerContext context)

        #region ModelDataLoadInserts(ContentCompilerContext context)
        /// <summary>
        /// This method inserts incoming data in to the model.
        /// </summary>
        /// <param name="context">The current context.</param>
        public override void ModelDataLoadInserts(ContentCompilerContext context)
        {
            Insert insert = null;
            try
            {
                insert = context.GetObjectPool<Insert>().Get();
                foreach (XmlElement element in context.ModelData.DataNodes())
                {
                    insert.Reset(element);
                    string status;
                    if (insert.ReferenceValue != null && insert.ReferenceValue != "")
                    {
                        Content outData = null;
                        try
                        {
                            throw new NotImplementedException();
                            //status = context.CDSHelper.Execute(insert.EntityType,
                            //    CDSData.Get(CDSAction.Read, insert.ReferenceType, insert.ReferenceValue),
                            //    out outData);

                            if (status != CH.HTTPCodes.OK_200 && !insert.PermitError)
                                throw new ContentCompilerException(
                                    string.Format("Data load error: {0}/{1}", insert.ReferenceType, insert.ReferenceValue));

                            insert.ResponseStatus = status;

                            if (outData != null)
                            {
                                XmlDocumentFragment frag = element.OwnerDocument.CreateDocumentFragment();
                                frag.InnerXml = ((XmlDocumentContentBase)outData).Payload.LastChild.OuterXml;
                                element.AppendChild(frag);
                            }
                            else if (!insert.PermitError)
                                throw new ArgumentNullException("CDS data is null.");

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            if (outData != null && outData.ObjectPoolCanReturn)
                                outData.ObjectPoolReturn();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (insert != null && insert.ObjectPoolCanReturn)
                    insert.ObjectPoolReturn();
            }
        }
        #endregion // ModelDataLoadInserts(ContentCompilerContext context)
    }

    #region Class --> Insert
    /// <summary>
    /// The insert class is used to insert compiled data in the the model data object.
    /// </summary>
    public class Insert : PoolableReturnableObjectBase
    {
        #region Declarations
        XmlElement node;
        #endregion // Declarations

        #region Constructor
        public Insert()
        {
            Reset();
        }
        #endregion // Internal Constructor

        #region Reset()
        /// <summary>
        /// This method resets the pbject to its default value.
        /// </summary>
        public override void Reset()
        {
            node = null;
        }
        #endregion // Reset()
        #region Reset(XmlElement element)
        /// <summary>
        /// This method resets the pbject to its default value.
        /// </summary>
        public virtual void Reset(XmlElement element)
        {
            node = element;
        }
        #endregion // Reset()

        #region ID
        /// <summary>
        /// The identifier
        /// </summary>
        public string ID
        {
            get { return node.Attributes["id"].InnerText; }
        }
        #endregion // ID

        #region ReferenceType
        /// <summary>
        /// The entity reference type.
        /// </summary>
        public string ReferenceType
        {
            get { return node.Attributes["reftype"].InnerText; }
        }
        #endregion // ReferenceType
        #region ReferenceValue
        /// <summary>
        /// The entity reference.
        /// </summary>
        public string ReferenceValue
        {
            get { return node.Attributes["refvalue"].InnerText; }
        }
        #endregion // Reference

        #region PermitError
        /// <summary>
        /// This property identifies whether the entity response error is permitted.
        /// </summary>
        public bool PermitError
        {
            get
            {
                return node.Attributes["permiterror"] != null ?
                    (node.Attributes["permiterror"].InnerText == "true") : false;
            }
        }
        #endregion // PermitError

        #region ResponseStatus
        /// <summary>
        /// This property gets or sets the response status for the data section.
        /// </summary>
        public string ResponseStatus
        {
            get
            {
                if (node == null || node.Attributes["responsestatus"] == null)
                    return null;
                return node.Attributes["responsestatus"].InnerText;
            }
            set
            {
                node.SetAttribute("responsestatus", value);
            }
        }
        #endregion // ResponseStatus

        #region EntityType
        /// <summary>
        /// The Entity Type
        /// </summary>
        public Type EntityType
        {
            get { return RH.CreateTypeFromString(node.Attributes["entitytype"].Value); }
        }
        #endregion // EntityType
    }
    #endregion // class Insert
}
