#region using
using System;
#endregion 
namespace Ximura.Framework
{
    /// <summary>
    /// This interface defines public methods for the finite state machine.
    /// </summary>
    public interface IXimuraFSM : IXimuraCommandBase, IXimuraCommandRQ
    {
        void ClearContextPool();

        bool ExternalStatesAllow { get; set; }
    }
}
