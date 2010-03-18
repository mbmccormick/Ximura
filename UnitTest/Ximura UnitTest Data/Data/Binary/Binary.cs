#region using
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ximura;
using Ximura.Data;
#endregion
namespace Ximura.UnitTest.Data
{
    public class Binary : XmlContentBase
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
