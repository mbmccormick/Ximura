#region using
using System;
using System.IO;
using System.Text;

using Ximura;
#endregion  
namespace Ximura
{
    /// <summary>
    /// This class is used to convert a byte stream in to a set of unicode characters.
    /// </summary>
    public class UnicodeCharEnumerator : ObjectEnumerator<Stream, char>
    {
        #region Unicode byte markers
        public static readonly byte[] ByteMarkerUTF8 = new byte[] { 0xEF, 0xBB, 0xBF };

        public static readonly byte[] ByteMarkerUTF16BE = new byte[] { 0xFE, 0xFF };
        public static readonly byte[] ByteMarkerUTF16LE = new byte[] { 0xFF, 0xFE };

        public static readonly byte[] ByteMarkerUTF32BE = new byte[] { 0x00, 0x00, 0xFE, 0xFF };
        public static readonly byte[] ByteMarkercnUTF32LE = new byte[] { 0xFF, 0xFE, 0x00, 0x00 };
        #endregion  

        #region Declarations
        private bool mPreambleRead;
        //The maximum number of bytes for a character.
        byte[] mBuffer = new byte[4];
        int mBufferPosition = 0;

        #endregion 
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="data">The stream to read from.</param>
        public UnicodeCharEnumerator(Stream data)
            : this(data, null){}
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="data">The stream to read from.</param>
        /// <param name="enc">The encoding type for the file.</param>
        public UnicodeCharEnumerator(Stream data, Encoding enc)
            : base(data, null) 
        {
            if (enc == null)
                Enc = Encoding.UTF8;
            else
                Enc = enc;

            //Set whether a preamble is required.
            byte[] preA = Enc.GetPreamble();
            mPreambleRead = preA != null && preA.Length > 0;
        }
        #endregion  

        #region Parse(Stream data)
        /// <summary>
        /// This override converts the bytes read from the stream in to characters.
        /// </summary>
        /// <param name="data"The stream to read from.</param>
        /// <returns>The tuple to return.</returns>
        protected override Tuple<char, Stream>? Parse(Stream data)
        {
            //Check that the stream can read the data.
            if (!data.CanRead)
                return null;

            int item;

            if (mPreambleRead)
            { 
                if (!SkipPreamble(data, out item))
                    return null;
            }
            else
                item = data.ReadByte();

            if (item == -1)
                return null;

            mBuffer[mBufferPosition] = (byte)item;
            mBufferPosition++;

            while (Enc.GetDecoder().GetCharCount(mBuffer, 0, mBufferPosition) == 0)
            {
                item = data.ReadByte();

                if (item == -1)
                    return null;

                mBuffer[mBufferPosition] = (byte)item;
                mBufferPosition++;
            }
            
            char[] cItems = Enc.GetChars(mBuffer, 0, mBufferPosition);
            mBufferPosition = 0;

            return new Tuple<char, Stream>(cItems[0], data);
        }
        #endregion  

        #region SkipPreamble(Stream data, out int item)
        /// <summary>
        /// This method skips any encoding preamble bytes at the start of the stream.
        /// </summary>
        /// <param name="data">The data stream.</param>
        /// <param name="item">The first character after the preamble.</param>
        /// <returns>Returns true if the enumeration can proceed. False if the end of the stream has been reached.</returns>
        private bool SkipPreamble(Stream data, out int item)
        {
            item = data.ReadByte();
            if (item == -1)
                return false;

            int pos = 0;
            byte[] preA = Enc.GetPreamble();
            //Skip any preamble bytes. We will not save these bytes for the moment.
            while (pos<preA.Length)
            {
                if (preA[pos] != (byte)item)
                {
                    mPreambleRead = false;
                    break;
                }

                item = data.ReadByte();
                if (item == -1)
                    return false;

                pos++;
            }

            return true;
        }
        #endregion  

        #region Enc
        /// <summary>
        /// This is the encoding used for the char enumerator.
        /// </summary>
        public Encoding Enc { get; private set; }
        #endregion  
    }
}
