#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Ximura;
using RH = Ximura.Helper.Reflection;
using Ximura.Data;
using Ximura.Communication;
#endregion
namespace Ximura.UnitTest.Communication
{
    /// <summary>
    /// This class is used to load a resource or a stream in to a message.
    /// </summary>
    /// <typeparam name="M">The message type.</typeparam>
    public class MessageLoad<M>
        where M : Message, new()
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor. This loads the resource in to the message.
        /// </summary>
        /// <param name="resource">The resource.</param>
        public MessageLoad(string resource)
        {
            using (Stream sResource = RH.ResourceLoadStream(resource))
            {
                Initialize(sResource);
            }
        }
        /// <summary>
        /// This is the default constructor. This loads the resource in to the message.
        /// </summary>
        /// <param name="resource">The resource stream.</param>
        public MessageLoad(Stream resource)
        {
            Initialize(resource);
        }
        #endregion 
        #region Initialize(Stream sData)
        /// <summary>
        /// This method initializes the message.
        /// </summary>
        /// <param name="sData">The stream data.</param>
        protected virtual void Initialize(Stream sData)
        {
            M tempMessage = new M();
            tempMessage.PoolManager = new PoolManager();
            byte[] blob = new byte[10000];

            tempMessage.Load();

            while (sData.CanRead)
            {
                int value = sData.Read(blob, 0, 1000);
                if (value == 0)
                    break;

                tempMessage.Write(blob, 0, value);
            }

            Data = tempMessage;
        }
        #endregion 

        #region Data
        /// <summary>
        /// This is the message.
        /// </summary>
        public M Data{get;set;}
        #endregion 
    }
}
