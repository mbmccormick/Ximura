#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ximura;
using Ximura.Data;
using Ximura.Persistence;
#endregion
namespace Ximura.Data.Persistence
{
    /// <summary>
    /// This interface is implemented by context objects that process persistence service requests.
    /// </summary>
    public interface IPersistenceContext
    {

        /// <summary>
        /// This property returns true if the request is by reference.
        /// </summary>
        bool RequestIsByReference { get; }

        /// <summary>
        /// This is the cache style.
        /// </summary>
        ContentCacheOptions CacheStyle { get; }

        /// <summary>
        /// This is the cache timeout in seconds.
        /// </summary>
        int CacheTimeOut { get; }

        /// <summary>
        /// This method initializes the context.
        /// </summary>
        void Initialize();

        /// <summary>
        /// This method maps the PMCapability request to its corresponding CDS Action type.
        /// </summary>
        /// <returns></returns>
        CDSStateAction CDSStateActionResolve();

        /// <summary>
        /// This method processes the specific CRUD execution plan for the request.
        /// </summary>
        bool CDSStateProcessDirective(CDSStateAction action);

        /// <summary>
        /// This method provides any clean up or finalization of the response.
        /// </summary>
        void Finish();

        /// <summary>
        /// This request returns true if the content is cacheable. 
        /// This will be used to indicate whether the Cache method should
        /// be called after the action has been completed.
        /// </summary>
        bool ContentIsCacheable { get; set; }

        /// <summary>
        /// This method checks whether the entity being passed is a fragment object.
        /// </summary>
        /// <param name="Request">The request containing the entity.</param>
        /// <returns>Returns true if the object is an entity.</returns>
        bool ContentIsFragment { get; }
    }
}
