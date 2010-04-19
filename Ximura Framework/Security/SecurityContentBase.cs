#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Net.Mail;

using Ximura;
using Ximura.Data;
#endregion // using
namespace Ximura.Security
{
    public abstract class SecurityContentBase: Content
    {
        public override int Load(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void OnDeserialization(object sender)
        {
            throw new NotImplementedException();
        }

        protected override byte[] ContentBody
        {
            get { throw new NotImplementedException(); }
        }


    }
}
