#region using
using System;
#endregion  
namespace Ximura
{
#if(SILVERLIGHT)
    /// <summary>
    /// This interface is added to replace the windows interface that is not present in the SILVERLIGHT core library.
    /// </summary>
    public interface ISupportInitialize
    {
        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        void BeginInit();
        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        void EndInit();
    }
#endif
}
