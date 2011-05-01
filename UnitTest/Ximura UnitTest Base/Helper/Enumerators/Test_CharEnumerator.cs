#region using
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ximura;
using System.IO;
#endregion // using
namespace Ximura.UnitTest
{
    [TestClass]
    public class Test_CharEnumerator
    {
        [TestMethod]
        public void TestMethod1()
        {
            byte[] blob = Encoding.UTF8.GetBytes("早 市 区 最 低 气 温 约 12 度 ， 打 鼓 岭 更 只 有 7 度 左 右\r\n");
            //byte[] blob = Encoding.UTF8.GetBytes("蘋果好介紹：白德民揀得真係\r\n");
            //byte[] blob = Encoding.UTF8.GetBytes("This is a test example using \tcertain control characters\r\n");
            char[] output;

            using(MemoryStream ms = new MemoryStream())
            {
                ms.Write(UnicodeCharEnumerator.ByteMarkerUTF8,0,3);
                ms.Write(blob,0,blob.Length);
                ms.Position = 0;

                UnicodeCharEnumerator uchEnum = new UnicodeCharEnumerator(ms);

                output = uchEnum.ToArray();
            }
        }
    }
}
