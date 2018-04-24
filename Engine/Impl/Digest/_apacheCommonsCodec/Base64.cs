using System;
using System.Text;

namespace ESS.FW.Bpm.Engine.Impl.Digest._apacheCommonsCodec
{
    /// <summary>
    ///     Provides Base64 encoding and decoding as defined by RFC 2045.
    ///     <para>
    ///         This class implements section <cite>6.8. Base64 Content-Transfer-Encoding</cite> from RFC 2045
    ///         <cite>
    ///             Multipurpose
    ///             Internet Mail Extensions (MIME) Part One: Format of Internet Message Bodies
    ///         </cite>
    ///         by Freed and Borenstein.
    ///     </para>
    ///     <para>
    ///         The class can be parameterized in the following manner with various constructors:
    ///         <ul>
    ///             <li>Uri-safe mode: Default off.</li>
    ///             <li>
    ///                 Line length: Default 76. Line length that aren't multiples of 4 will still essentially end up being
    ///                 multiples of
    ///                 4 in the encoded data.
    ///                 <li>Line separator: Default is CRLF ("\r\n")</li>
    ///         </ul>
    ///     </para>
    ///     <para>
    ///         Since this class operates directly on byte streams, and not character streams, it is hard-coded to only
    ///         encode/decode
    ///         character encodings which are compatible with the lower 127 ASCII chart (ISO-8859-1, Windows-1252, UTF-8, etc).
    ///     </para>
    /// </summary>
    /// <seealso cref=
    /// <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045</a>
    ///  Apache Software Foundation
    /// @since 1.0
    /// @version $Id: Base64.java 801706 2009-08-06 16:27:06Z niallp $
    /// </seealso>
    public class Base64
    {
        private const int DefaultBufferResizeFactor = 2;

        private const int DefaultBufferSize = 8192;

        /// <summary>
        ///     Chunk size per RFC 2045 section 6.8.
        ///     <para>
        ///         The {@value} character limit does not count the trailing CRLF, but counts all other characters, including any
        ///         equal signs.
        ///     </para>
        /// </summary>
        /// <seealso cref=
        /// <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045 section 6.8</a>
        /// </seealso>
        internal const int ChunkSize = 76;

        /// <summary>
        ///     Byte used to pad output.
        /// </summary>
        private const byte Pad = (byte) '=';

        /// <summary>
        ///     Mask used to extract 6 bits, used when encoding
        /// </summary>
        private const int Mask_6Bits = 0x3f;

        /// <summary>
        ///     Mask used to extract 8 bits, used in decoding base64 bytes
        /// </summary>
        private const int Mask_8Bits = 0xff;

        /// <summary>
        ///     Chunk separator per RFC 2045 section 2.1.
        ///     <para>
        ///         N.B. The next major release may break compatibility and make this field private.
        ///     </para>
        /// </summary>
        /// <seealso cref=
        /// <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045 section 2.1</a>
        /// </seealso>
        internal static readonly byte[] ChunkSeparator = {(byte) '\r', (byte) '\n'};

        /// <summary>
        ///     This array is a lookup table that translates 6-bit positive integer index values into their "Base64 Alphabet"
        ///     equivalents as specified in Table 1 of RFC 2045.
        ///     Thanks to "commons" project in ws.apache.org for this code.
        ///     http://svn.apache.org/repos/asf/webservices/commons/trunk/modules/util/
        /// </summary>
        private static readonly byte[] StandardEncodeTable =
        {
            (byte) 'A', (byte) 'B', (byte) 'C', (byte) 'D',
            (byte) 'E', (byte) 'F', (byte) 'G', (byte) 'H', (byte) 'I', (byte) 'J', (byte) 'K', (byte) 'L',
            (byte) 'M', (byte) 'N', (byte) 'O', (byte) 'P', (byte) 'Q', (byte) 'R', (byte) 'S', (byte) 'T',
            (byte) 'U', (byte) 'V', (byte) 'W', (byte) 'X', (byte) 'Y', (byte) 'Z', (byte) 'a', (byte) 'b',
            (byte) 'c', (byte) 'd', (byte) 'e', (byte) 'f', (byte) 'g', (byte) 'h', (byte) 'i', (byte) 'j',
            (byte) 'k', (byte) 'l', (byte) 'm', (byte) 'n', (byte) 'o', (byte) 'p', (byte) 'q', (byte) 'r',
            (byte) 's', (byte) 't', (byte) 'u', (byte) 'v', (byte) 'w', (byte) 'x', (byte) 'y', (byte) 'z',
            (byte) '0', (byte) '1', (byte) '2', (byte) '3', (byte) '4', (byte) '5', (byte) '6', (byte) '7',
            (byte) '8', (byte) '9', (byte) '+', (byte) '/'
        };

        /// <summary>
        ///     This is a copy of the STANDARD_ENCODE_TABLE above, but with + and /
        ///     changed to - and _ to make the encoded Base64 results more Uri-SAFE.
        ///     This table is only used when the Base64's mode is set to Uri-SAFE.
        /// </summary>
        private static readonly byte[] UrlSafeEncodeTable =
        {
            (byte) 'A', (byte) 'B', (byte) 'C', (byte) 'D',
            (byte) 'E', (byte) 'F', (byte) 'G', (byte) 'H', (byte) 'I', (byte) 'J', (byte) 'K', (byte) 'L',
            (byte) 'M', (byte) 'N', (byte) 'O', (byte) 'P', (byte) 'Q', (byte) 'R', (byte) 'S', (byte) 'T',
            (byte) 'U', (byte) 'V', (byte) 'W', (byte) 'X', (byte) 'Y', (byte) 'Z', (byte) 'a', (byte) 'b',
            (byte) 'c', (byte) 'd', (byte) 'e', (byte) 'f', (byte) 'g', (byte) 'h', (byte) 'i', (byte) 'j',
            (byte) 'k', (byte) 'l', (byte) 'm', (byte) 'n', (byte) 'o', (byte) 'p', (byte) 'q', (byte) 'r',
            (byte) 's', (byte) 't', (byte) 'u', (byte) 'v', (byte) 'w', (byte) 'x', (byte) 'y', (byte) 'z',
            (byte) '0', (byte) '1', (byte) '2', (byte) '3', (byte) '4', (byte) '5', (byte) '6', (byte) '7',
            (byte) '8', (byte) '9', (byte) '-', (byte) '_'
        };

        /// <summary>
        ///     This array is a lookup table that translates Unicode characters drawn from the "Base64 Alphabet" (as specified in
        ///     Table 1 of RFC 2045) into their 6-bit positive integer equivalents. Characters that are not in the Base64
        ///     alphabet but fall within the bounds of the array are translated to -1.
        ///     Note: '+' and '-' both decode to 62. '/' and '_' both decode to 63. This means decoder seamlessly handles both
        ///     URL_SAFE and STANDARD base64. (The encoder, on the other hand, needs to know ahead of time what to emit).
        ///     Thanks to "commons" project in ws.apache.org for this code.
        ///     http://svn.apache.org/repos/asf/webservices/commons/trunk/modules/util/
        /// </summary>
        private static readonly byte[] DecodeTable =
        {
            //-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            //-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            //62, -1, 62, -1, 63, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1, -1, 0, 1, 2, 3, 4, 5, 6,
            //7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, 63, -1, 26, 27, 28,
            29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51
        };

        /// <summary>
        ///     Convenience variable to help us determine when our buffer is going to run out of room and needs resizing.
        ///     <code>decodeSize = 3 + lineSeparator.length;</code>
        /// </summary>
        private readonly int _decodeSize;

        /// <summary>
        ///     Convenience variable to help us determine when our buffer is going to run out of room and needs resizing.
        ///     <code>encodeSize = 4 + lineSeparator.length;</code>
        /// </summary>
        private readonly int _encodeSize;

        // The static final fields above are used for the original static byte[] methods on Base64.
        // The private member fields below are used with the new streaming approach, which requires
        // some state be preserved between calls of encode() and decode().

        /// <summary>
        ///     Encode table to use: either STANDARD or URL_SAFE. Note: the DECODE_TABLE above remains static because it is able
        ///     to decode both STANDARD and URL_SAFE streams, but the encodeTable must be a member variable so we can switch
        ///     between the two modes.
        /// </summary>
        private readonly byte[] _encodeTable;

        /// <summary>
        ///     Line length for encoding. Not used when decoding. A value of zero or less implies no chunking of the base64
        ///     encoded data.
        /// </summary>
        private readonly int _lineLength;

        /// <summary>
        ///     Line separator for encoding. Not used when decoding. Only used if lineLength > 0.
        /// </summary>
        private readonly byte[] _lineSeparator;

        /// <summary>
        ///     Buffer for streaming.
        /// </summary>
        private byte[] _buffer;

        /// <summary>
        ///     Variable tracks how many characters have been written to the current line. Only used when encoding. We use it to
        ///     make sure each encoded line never goes beyond lineLength (if lineLength > 0).
        /// </summary>
        private int _currentLinePos;

        /// <summary>
        ///     Boolean flag to indicate the EOF has been reached. Once EOF has been reached, this Base64 object becomes useless,
        ///     and must be thrown away.
        /// </summary>
        private bool _eof;

        /// <summary>
        ///     Writes to the buffer only occur after every 3 reads when encoding, an every 4 reads when decoding. This variable
        ///     helps track that.
        /// </summary>
        private int _modulus;

        /// <summary>
        ///     Position where next character should be written in the buffer.
        /// </summary>
        private int _pos;

        /// <summary>
        ///     Position where next character should be read from the buffer.
        /// </summary>
        private int _readPos;

        /// <summary>
        ///     Place holder for the 3 bytes we're dealing with for our base64 logic. Bitwise operations store and extract the
        ///     base64 encoding or decoding from this variable.
        /// </summary>
        private int _x;

        /// <summary>
        ///     Creates a Base64 codec used for decoding (all modes) and encoding in Uri-unsafe mode.
        ///     <para>
        ///         When encoding the line length is 76, the line separator is CRLF, and the encoding table is
        ///         STANDARD_ENCODE_TABLE.
        ///     </para>
        ///     <para>
        ///         When decoding all variants are supported.
        ///     </para>
        /// </summary>
        public Base64() : this(false)
        {
        }

        /// <summary>
        ///     Creates a Base64 codec used for decoding (all modes) and encoding in the given Uri-safe mode.
        ///     <para>
        ///         When encoding the line length is 76, the line separator is CRLF, and the encoding table is
        ///         STANDARD_ENCODE_TABLE.
        ///     </para>
        ///     <para>
        ///         When decoding all variants are supported.
        ///     </para>
        /// </summary>
        /// <param name="urlSafe">
        ///     if <code>true</code>, Uri-safe encoding is used. In most cases this should be set to
        ///     <code>false</code>.
        ///     @since 1.4
        /// </param>
        public Base64(bool urlSafe) : this(ChunkSize, ChunkSeparator, urlSafe)
        {
        }

        /// <summary>
        ///     Creates a Base64 codec used for decoding (all modes) and encoding in Uri-unsafe mode.
        ///     <para>
        ///         When encoding the line length is given in the constructor, the line separator is CRLF, and the encoding table
        ///         is
        ///         STANDARD_ENCODE_TABLE.
        ///     </para>
        ///     <para>
        ///         Line lengths that aren't multiples of 4 will still essentially end up being multiples of 4 in the encoded data.
        ///     </para>
        ///     <para>
        ///         When decoding all variants are supported.
        ///     </para>
        /// </summary>
        /// <param name="lineLength">
        ///     Each line of encoded data will be at most of the given length (rounded down to nearest multiple of 4).
        ///     If lineLength <= 0, then the output will not be divided into lines (chunks). Ignored when decoding.
        ///     @since 1.4 </param>
        public Base64(int lineLength) : this(lineLength, ChunkSeparator)
        {
        }

        /// <summary>
        ///     Creates a Base64 codec used for decoding (all modes) and encoding in Uri-unsafe mode.
        ///     <para>
        ///         When encoding the line length and line separator are given in the constructor, and the encoding table is
        ///         STANDARD_ENCODE_TABLE.
        ///     </para>
        ///     <para>
        ///         Line lengths that aren't multiples of 4 will still essentially end up being multiples of 4 in the encoded data.
        ///     </para>
        ///     <para>
        ///         When decoding all variants are supported.
        ///     </para>
        /// </summary>
        /// <param name="lineLength">
        ///     Each line of encoded data will be at most of the given length (rounded down to nearest multiple of 4).
        ///     If lineLength <= 0, then the output will not be divided into lines (chunks). Ignored when decoding. </param>
        /// <param name="lineSeparator">
        ///     Each line of encoded data will end with this sequence of bytes.
        /// </param>
        /// <exception cref="IllegalArgumentException">
        ///     Thrown when the provided lineSeparator included some base64 characters.
        ///     @since 1.4
        /// </exception>
        public Base64(int lineLength, byte[] lineSeparator) : this(lineLength, lineSeparator, false)
        {
        }

        /// <summary>
        ///     Creates a Base64 codec used for decoding (all modes) and encoding in Uri-unsafe mode.
        ///     <para>
        ///         When encoding the line length and line separator are given in the constructor, and the encoding table is
        ///         STANDARD_ENCODE_TABLE.
        ///     </para>
        ///     <para>
        ///         Line lengths that aren't multiples of 4 will still essentially end up being multiples of 4 in the encoded data.
        ///     </para>
        ///     <para>
        ///         When decoding all variants are supported.
        ///     </para>
        /// </summary>
        /// <param name="lineLength">
        ///     Each line of encoded data will be at most of the given length (rounded down to nearest multiple of 4).
        ///     If lineLength <= 0, then the output will not be divided into lines (chunks). Ignored when decoding. </param>
        /// <param name="lineSeparator">
        ///     Each line of encoded data will end with this sequence of bytes.
        /// </param>
        /// <param name="urlSafe">
        ///     Instead of emitting '+' and '/' we emit '-' and '_' respectively. urlSafe is only applied to encode
        ///     operations. Decoding seamlessly handles both modes.
        /// </param>
        /// <exception cref="IllegalArgumentException">
        ///     The provided lineSeparator included some base64 characters. That's not going to work!
        ///     @since 1.4
        /// </exception>
        public Base64(int lineLength, byte[] lineSeparator, bool urlSafe)
        {
            if (lineSeparator == null)
            {
                lineLength = 0; // disable chunk-separating
                lineSeparator = ChunkSeparator; // this just gets ignored
            }
            this._lineLength = lineLength > 0 ? lineLength/4*4 : 0;
            this._lineSeparator = new byte[lineSeparator.Length];
            Array.Copy(lineSeparator, 0, this._lineSeparator, 0, lineSeparator.Length);
            if (lineLength > 0)
                _encodeSize = 4 + lineSeparator.Length;
            else
                _encodeSize = 4;
            _decodeSize = _encodeSize - 1;
            if (ContainsBase64Byte(lineSeparator))
            {
                var sep = StringUtils.NewStringUtf8(lineSeparator);
                throw new ArgumentException("lineSeperator must not contain base64 characters: [" + sep + "]");
            }
            _encodeTable = urlSafe ? UrlSafeEncodeTable : StandardEncodeTable;
        }

        /// <summary>
        ///     Returns our current encode mode. True if we're Uri-SAFE, false otherwise.
        /// </summary>
        /// <returns>
        ///     true if we're in Uri-SAFE mode, false otherwise.
        ///     @since 1.4
        /// </returns>
        public virtual bool UrlSafe
        {
            get { return _encodeTable == UrlSafeEncodeTable; }
        }

        /// <summary>
        ///     Returns true if this Base64 object has buffered data for reading.
        /// </summary>
        /// <returns> true if there is Base64 object still available for reading. </returns>
        internal virtual bool HasData()
        {
            return _buffer != null;
        }

        /// <summary>
        ///     Returns the amount of buffered data available for reading.
        /// </summary>
        /// <returns> The amount of buffered data available for reading. </returns>
        internal virtual int Avail()
        {
            return _buffer != null ? _pos - _readPos : 0;
        }

        /// <summary>
        ///     Doubles our buffer.
        /// </summary>
        private void ResizeBuffer()
        {
            if (_buffer == null)
            {
                _buffer = new byte[DefaultBufferSize];
                _pos = 0;
                _readPos = 0;
            }
            else
            {
                var b = new byte[_buffer.Length*DefaultBufferResizeFactor];
                Array.Copy(_buffer, 0, b, 0, _buffer.Length);
                _buffer = b;
            }
        }

        /// <summary>
        ///     Extracts buffered data into the provided byte[] array, starting at position bPos, up to a maximum of bAvail
        ///     bytes. Returns how many bytes were actually extracted.
        /// </summary>
        /// <param name="b">
        ///     byte[] array to extract the buffered data into.
        /// </param>
        /// <param name="bPos">
        ///     position in byte[] array to start extraction at.
        /// </param>
        /// <param name="bAvail">
        ///     amount of bytes we're allowed to extract. We may extract fewer (if fewer are available).
        /// </param>
        /// <returns> The number of bytes successfully extracted into the provided byte[] array. </returns>
        internal virtual int ReadResults(byte[] b, int bPos, int bAvail)
        {
            if (_buffer != null)
            {
                var len = Math.Min(Avail(), bAvail);
                if (_buffer != b)
                {
                    Array.Copy(_buffer, _readPos, b, bPos, len);
                    _readPos += len;
                    if (_readPos >= _pos)
                        _buffer = null;
                }
                else
                {
                    // Re-using the original consumer's output array is only
                    // allowed for one round.
                    _buffer = null;
                }
                return len;
            }
            return _eof ? -1 : 0;
        }

        /// <summary>
        ///     Sets the streaming buffer. This is a small optimization where we try to buffer directly to the consumer's output
        ///     array for one round (if the consumer calls this method first) instead of starting our own buffer.
        /// </summary>
        /// <param name="out">
        ///     byte[] array to buffer directly to.
        /// </param>
        /// <param name="outPos">
        ///     Position to start buffering into.
        /// </param>
        /// <param name="outAvail">
        ///     Amount of bytes available for direct buffering.
        /// </param>
        internal virtual void SetInitialBuffer(byte[] @out, int outPos, int outAvail)
        {
            // We can re-use consumer's original output array under
            // special circumstances, saving on some System.arraycopy().
            if ((@out != null) && (@out.Length == outAvail))
            {
                _buffer = @out;
                _pos = outPos;
                _readPos = outPos;
            }
        }

        /// <summary>
        ///     <para>
        ///         Encodes all of the provided data, starting at inPos, for inAvail bytes. Must be called at least twice: once
        ///         with
        ///         the data to encode, and once with inAvail set to "-1" to alert encoder that EOF has been reached, so flush last
        ///         remaining bytes (if not multiple of 3).
        ///     </para>
        ///     <para>
        ///         Thanks to "commons" project in ws.apache.org for the bitwise operations, and general approach.
        ///         http://svn.apache.org/repos/asf/webservices/commons/trunk/modules/util/
        ///     </para>
        /// </summary>
        /// <param name="in">
        ///     byte[] array of binary data to base64 encode.
        /// </param>
        /// <param name="inPos">
        ///     Position to start reading data from.
        /// </param>
        /// <param name="inAvail">
        ///     Amount of bytes available from input for encoding.
        /// </param>
        internal virtual void Encode(byte[] @in, int inPos, int inAvail)
        {
            if (_eof)
                return;
            // inAvail < 0 is how we're informed of EOF in the underlying data we're
            // encoding.
            if (inAvail < 0)
            {
                _eof = true;
                if ((_buffer == null) || (_buffer.Length - _pos < _encodeSize))
                    ResizeBuffer();
                switch (_modulus)
                {
                    case 1:
                        _buffer[_pos++] = _encodeTable[(_x >> 2) & Mask_6Bits];
                        _buffer[_pos++] = _encodeTable[(_x << 4) & Mask_6Bits];
                        // Uri-SAFE skips the padding to further reduce size.
                        if (_encodeTable == StandardEncodeTable)
                        {
                            _buffer[_pos++] = Pad;
                            _buffer[_pos++] = Pad;
                        }
                        break;

                    case 2:
                        _buffer[_pos++] = _encodeTable[(_x >> 10) & Mask_6Bits];
                        _buffer[_pos++] = _encodeTable[(_x >> 4) & Mask_6Bits];
                        _buffer[_pos++] = _encodeTable[(_x << 2) & Mask_6Bits];
                        // Uri-SAFE skips the padding to further reduce size.
                        if (_encodeTable == StandardEncodeTable)
                            _buffer[_pos++] = Pad;
                        break;
                }
                if ((_lineLength > 0) && (_pos > 0))
                {
                    Array.Copy(_lineSeparator, 0, _buffer, _pos, _lineSeparator.Length);
                    _pos += _lineSeparator.Length;
                }
            }
            else
            {
                for (var i = 0; i < inAvail; i++)
                {
                    if ((_buffer == null) || (_buffer.Length - _pos < _encodeSize))
                        ResizeBuffer();
                    _modulus = ++_modulus%3;
                    int b = @in[inPos++];
                    if (b < 0)
                        b += 256;
                    _x = (_x << 8) + b;
                    if (0 == _modulus)
                    {
                        _buffer[_pos++] = _encodeTable[(_x >> 18) & Mask_6Bits];
                        _buffer[_pos++] = _encodeTable[(_x >> 12) & Mask_6Bits];
                        _buffer[_pos++] = _encodeTable[(_x >> 6) & Mask_6Bits];
                        _buffer[_pos++] = _encodeTable[_x & Mask_6Bits];
                        _currentLinePos += 4;
                        if ((_lineLength > 0) && (_lineLength <= _currentLinePos))
                        {
                            Array.Copy(_lineSeparator, 0, _buffer, _pos, _lineSeparator.Length);
                            _pos += _lineSeparator.Length;
                            _currentLinePos = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     <para>
        ///         Decodes all of the provided data, starting at inPos, for inAvail bytes. Should be called at least twice: once
        ///         with the data to decode, and once with inAvail set to "-1" to alert decoder that EOF has been reached. The "-1"
        ///         call is not necessary when decoding, but it doesn't hurt, either.
        ///     </para>
        ///     <para>
        ///         Ignores all non-base64 characters. This is how chunked (e.g. 76 character) data is handled, since CR and LF are
        ///         silently ignored, but has implications for other bytes, too. This method subscribes to the garbage-in,
        ///         garbage-out philosophy: it will not check the provided data for validity.
        ///     </para>
        ///     <para>
        ///         Thanks to "commons" project in ws.apache.org for the bitwise operations, and general approach.
        ///         http://svn.apache.org/repos/asf/webservices/commons/trunk/modules/util/
        ///     </para>
        /// </summary>
        /// <param name="in">
        ///     byte[] array of ascii data to base64 decode.
        /// </param>
        /// <param name="inPos">
        ///     Position to start reading data from.
        /// </param>
        /// <param name="inAvail">
        ///     Amount of bytes available from input for encoding.
        /// </param>
        internal virtual void Decode(byte[] @in, int inPos, int inAvail)
        {
            if (_eof)
                return;
            if (inAvail < 0)
                _eof = true;
            for (var i = 0; i < inAvail; i++)
            {
                if ((_buffer == null) || (_buffer.Length - _pos < _decodeSize))
                    ResizeBuffer();
                var b = @in[inPos++];
                if (b == Pad)
                {
                    // We're done.
                    _eof = true;
                    break;
                }
                if ((b >= 0) && (b < DecodeTable.Length))
                {
                    int result = DecodeTable[b];
                    if (result >= 0)
                    {
                        _modulus = ++_modulus%4;
                        _x = (_x << 6) + result;
                        if (_modulus == 0)
                        {
                            _buffer[_pos++] = (byte) ((_x >> 16) & Mask_8Bits);
                            _buffer[_pos++] = (byte) ((_x >> 8) & Mask_8Bits);
                            _buffer[_pos++] = (byte) (_x & Mask_8Bits);
                        }
                    }
                }
            }

            // Two forms of EOF as far as base64 decoder is concerned: actual
            // EOF (-1) and first time '=' character is encountered in stream.
            // This approach makes the '=' padding characters completely optional.
            if (_eof && (_modulus != 0))
            {
                _x = _x << 6;
                switch (_modulus)
                {
                    case 2:
                        _x = _x << 6;
                        _buffer[_pos++] = (byte) ((_x >> 16) & Mask_8Bits);
                        break;
                    case 3:
                        _buffer[_pos++] = (byte) ((_x >> 16) & Mask_8Bits);
                        _buffer[_pos++] = (byte) ((_x >> 8) & Mask_8Bits);
                        break;
                }
            }
        }

        /// <summary>
        ///     Returns whether or not the <code>octet</code> is in the base 64 alphabet.
        /// </summary>
        /// <param name="octet">
        ///     The value to test
        /// </param>
        /// <returns>
        ///     <code>true</code> if the value is defined in the the base 64 alphabet, <code>false</code> otherwise.
        ///     @since 1.4
        /// </returns>
        public static bool IsBase64(byte octet)
        {
            return (octet == Pad) || ((octet >= 0) && (octet < DecodeTable.Length) && (DecodeTable[octet] != -1));
        }

        /// <summary>
        ///     Tests a given byte array to see if it contains only valid characters within the Base64 alphabet. Currently the
        ///     method treats whitespace as valid.
        /// </summary>
        /// <param name="arrayOctet">
        ///     byte array to test
        /// </param>
        /// <returns>
        ///     <code>true</code> if all bytes are valid characters in the Base64 alphabet or if the byte array is empty;
        ///     false, otherwise
        /// </returns>
        public static bool IsArrayByteBase64(byte[] arrayOctet)
        {
            for (var i = 0; i < arrayOctet.Length; i++)
                if (!IsBase64(arrayOctet[i]) && !IsWhiteSpace(Convert.ToChar(arrayOctet[i])))
                    return false;
            return true;
        }

        /// <summary>
        ///     Tests a given byte array to see if it contains only valid characters within the Base64 alphabet.
        /// </summary>
        /// <param name="arrayOctet">
        ///     byte array to test
        /// </param>
        /// <returns> <code>true</code> if any byte is a valid character in the Base64 alphabet; false herwise </returns>
        private static bool ContainsBase64Byte(byte[] arrayOctet)
        {
            for (var i = 0; i < arrayOctet.Length; i++)
                if (IsBase64(arrayOctet[i]))
                    return true;
            return false;
        }

        /// <summary>
        ///     Encodes binary data using the base64 algorithm but does not chunk the output.
        /// </summary>
        /// <param name="binaryData">
        ///     binary data to encode
        /// </param>
        /// <returns> byte[] containing Base64 characters in their UTF-8 representation. </returns>
        public static byte[] EncodeBase64(byte[] binaryData)
        {
            return EncodeBase64(binaryData, false);
        }

        /// <summary>
        ///     Encodes binary data using the base64 algorithm into 76 character blocks separated by CRLF.
        /// </summary>
        /// <param name="binaryData">
        ///     binary data to encode
        /// </param>
        /// <returns>
        ///     String containing Base64 characters.
        ///     @since 1.4
        /// </returns>
        public static string EncodeBase64String(byte[] binaryData)
        {
            throw new NotImplementedException();
            //return StringUtils.NewStringUtf8(EncodeBase64(binaryData, true));
        }

        /// <summary>
        ///     Encodes binary data using a Uri-safe variation of the base64 algorithm but does not chunk the output. The
        ///     url-safe variation emits - and _ instead of + and / characters.
        /// </summary>
        /// <param name="binaryData">
        ///     binary data to encode
        /// </param>
        /// <returns>
        ///     byte[] containing Base64 characters in their UTF-8 representation.
        ///     @since 1.4
        /// </returns>
        public static byte[] EncodeBase64UrlSafe(byte[] binaryData)
        {
            return EncodeBase64(binaryData, false, true);
        }

        /// <summary>
        ///     Encodes binary data using a Uri-safe variation of the base64 algorithm but does not chunk the output. The
        ///     url-safe variation emits - and _ instead of + and / characters.
        /// </summary>
        /// <param name="binaryData">
        ///     binary data to encode
        /// </param>
        /// <returns>
        ///     String containing Base64 characters
        ///     @since 1.4
        /// </returns>
        public static string EncodeBase64UrlSafeString(byte[] binaryData)
        {
            return StringUtils.NewStringUtf8(EncodeBase64(binaryData, false, true));
        }

        /// <summary>
        ///     Encodes binary data using the base64 algorithm and chunks the encoded output into 76 character blocks
        /// </summary>
        /// <param name="binaryData">
        ///     binary data to encode
        /// </param>
        /// <returns> Base64 characters chunked in 76 character blocks </returns>
        public static byte[] EncodeBase64Chunked(byte[] binaryData)
        {
            return EncodeBase64(binaryData, true);
        }

        /// <summary>
        ///     Decodes an Object using the base64 algorithm. This method is provided in order to satisfy the requirements of the
        ///     Decoder interface, and will throw a DecoderException if the supplied object is not of type byte[] or String.
        /// </summary>
        /// <param name="pObject">
        ///     Object to decode
        /// </param>
        /// <returns> An object (of type byte[]) containing the binary data which corresponds to the byte[] or String supplied. </returns>
        /// <exception cref="DecoderException">
        ///     if the parameter supplied is not of type byte[]
        /// </exception>
        public virtual object Decode(object pObject)
        {
            if (pObject is byte[])
                return Decode((byte[]) pObject);
            if (pObject is string)
                return Decode((string) pObject);
            throw new System.Exception("Parameter supplied to Base64 decode is not a byte[] or a String");
        }

        /// <summary>
        ///     Decodes a String containing containing characters in the Base64 alphabet.
        /// </summary>
        /// <param name="pArray">
        ///     A String containing Base64 character data
        /// </param>
        /// <returns>
        ///     a byte array containing binary data
        ///     @since 1.4
        /// </returns>
        public virtual byte[] Decode(string pArray)
        {
            return System.Text.Encoding.UTF8.GetBytes(pArray);
            //return Decode(StringUtils.GetBytesUtf8(pArray));
        }

        /// <summary>
        ///     Decodes a byte[] containing containing characters in the Base64 alphabet.
        /// </summary>
        /// <param name="pArray">
        ///     A byte array containing Base64 character data
        /// </param>
        /// <returns> a byte array containing binary data </returns>
        public virtual byte[] Decode(byte[] pArray)
        {
            Reset();
            if ((pArray == null) || (pArray.Length == 0))
                return pArray;
            long len = pArray.Length*3/4;
            var buf = new byte[(int) len];
            SetInitialBuffer(buf, 0, buf.Length);
            Decode(pArray, 0, pArray.Length);
            Decode(pArray, 0, -1); // Notify decoder of EOF.

            // Would be nice to just return buf (like we sometimes do in the encode
            // logic), but we have no idea what the line-length was (could even be
            // variable).  So we cannot determine ahead of time exactly how big an
            // array is necessary.  Hence the need to construct a 2nd byte array to
            // hold the final result:

            var result = new byte[_pos];
            ReadResults(result, 0, result.Length);
            return result;
        }

        /// <summary>
        ///     Encodes binary data using the base64 algorithm, optionally chunking the output into 76 character blocks.
        /// </summary>
        /// <param name="binaryData">
        ///     Array containing binary data to encode.
        /// </param>
        /// <param name="isChunked">
        ///     if <code>true</code> this encoder will chunk the base64 output into 76 character blocks
        /// </param>
        /// <returns> Base64-encoded data. </returns>
        /// <exception cref="IllegalArgumentException">
        ///     Thrown when the input array needs an output array bigger than <seealso cref="Integer#MAX_VALUE" />
        /// </exception>
        public static byte[] EncodeBase64(byte[] binaryData, bool isChunked)
        {
            return EncodeBase64(binaryData, isChunked, false);
        }

        /// <summary>
        ///     Encodes binary data using the base64 algorithm, optionally chunking the output into 76 character blocks.
        /// </summary>
        /// <param name="binaryData">
        ///     Array containing binary data to encode.
        /// </param>
        /// <param name="isChunked">
        ///     if <code>true</code> this encoder will chunk the base64 output into 76 character blocks
        /// </param>
        /// <param name="urlSafe">
        ///     if <code>true</code> this encoder will emit - and _ instead of the usual + and / characters.
        /// </param>
        /// <returns> Base64-encoded data. </returns>
        /// <exception cref="IllegalArgumentException">
        ///     Thrown when the input array needs an output array bigger than <seealso cref="Integer#MAX_VALUE" />
        ///     @since 1.4
        /// </exception>
        public static byte[] EncodeBase64(byte[] binaryData, bool isChunked, bool urlSafe)
        {
            return EncodeBase64(binaryData, isChunked, urlSafe, int.MaxValue);
        }

        /// <summary>
        ///     Encodes binary data using the base64 algorithm, optionally chunking the output into 76 character blocks.
        /// </summary>
        /// <param name="binaryData">
        ///     Array containing binary data to encode.
        /// </param>
        /// <param name="isChunked">
        ///     if <code>true</code> this encoder will chunk the base64 output into 76 character blocks
        /// </param>
        /// <param name="urlSafe">
        ///     if <code>true</code> this encoder will emit - and _ instead of the usual + and / characters.
        /// </param>
        /// <param name="maxResultSize">
        ///     The maximum result size to accept.
        /// </param>
        /// <returns> Base64-encoded data. </returns>
        /// <exception cref="IllegalArgumentException">
        ///     Thrown when the input array needs an output array bigger than maxResultSize
        ///     @since 1.4
        /// </exception>
        public static byte[] EncodeBase64(byte[] binaryData, bool isChunked, bool urlSafe, int maxResultSize)
        {
            if ((binaryData == null) || (binaryData.Length == 0))
                return binaryData;

            var len = GetEncodeLength(binaryData, ChunkSize, ChunkSeparator);
            if (len > maxResultSize)
                throw new ArgumentException("Input array too big, the output array would be bigger (" + len +
                                            ") than the specified maxium size of " + maxResultSize);

            var b64 = isChunked ? new Base64(urlSafe) : new Base64(0, ChunkSeparator, urlSafe);
            return b64.Encode(binaryData);
        }

        /// <summary>
        ///     Decodes a Base64 String into octets
        /// </summary>
        /// <param name="base64String">
        ///     String containing Base64 data
        /// </param>
        /// <returns>
        ///     Array containing decoded data.
        ///     @since 1.4
        /// </returns>
        public static byte[] DecodeBase64(string base64String)
        {
            return new Base64().Decode(base64String);
        }

        /// <summary>
        ///     Decodes Base64 data into octets
        /// </summary>
        /// <param name="base64Data">
        ///     Byte array containing Base64 data
        /// </param>
        /// <returns> Array containing decoded data. </returns>
        public static byte[] DecodeBase64(byte[] base64Data)
        {
            return new Base64().Decode(base64Data);
        }

        /// <summary>
        ///     Discards any whitespace from a base-64 encoded block.
        /// </summary>
        /// <param name="data">
        ///     The base-64 encoded data to discard the whitespace from.
        /// </param>
        /// <returns> The data, less whitespace (see RFC 2045). </returns>
        /// @deprecated This method is no longer needed
        internal static char[] DiscardWhitespace(char[] data)
        {
            var groomedData = new char[data.Length];
            var bytesCopied = 0;
            for (var i = 0; i < data.Length; i++)
                switch (data[i])
                {
                    case ' ':
                    case '\n':
                    case '\r':
                    case '\t':
                        break;
                    default:
                        groomedData[bytesCopied++] = data[i];
                        break;
                }
            var packedData = new char[bytesCopied];
            Array.Copy(groomedData, 0, packedData, 0, bytesCopied);
            return packedData;
        }

        /// <summary>
        ///     Checks if a byte value is whitespace or not.
        /// </summary>
        /// <param name="byteToCheck">
        ///     the byte to check
        /// </param>
        /// <returns> true if byte is whitespace, false otherwise </returns>
        private static bool IsWhiteSpace(char byteToCheck)
        {
            switch (byteToCheck)
            {
                case ' ':
                case '\n':
                case '\r':
                case '\t':
                    return true;
                default:
                    return false;
            }
        }

        // Implementation of the Encoder Interface

        /// <summary>
        ///     Encodes an Object using the base64 algorithm. This method is provided in order to satisfy the requirements of the
        ///     Encoder interface, and will throw an EncoderException if the supplied object is not of type byte[].
        /// </summary>
        /// <param name="pObject">
        ///     Object to encode
        /// </param>
        /// <returns> An object (of type byte[]) containing the base64 encoded data which corresponds to the byte[] supplied. </returns>
        /// <exception cref="EncoderException">
        ///     if the parameter supplied is not of type byte[]
        /// </exception>
        public virtual object Encode(object pObject)
        {
            if (!(pObject is byte[]))
                throw new System.Exception("Parameter supplied to Base64 encode is not a byte[]");
            return Encode((byte[]) pObject);
        }

        /// <summary>
        ///     Encodes a byte[] containing binary data, into a String containing characters in the Base64 alphabet.
        /// </summary>
        /// <param name="pArray">
        ///     a byte array containing binary data
        /// </param>
        /// <returns>
        ///     A String containing only Base64 character data
        ///     @since 1.4
        /// </returns>
        public virtual string EncodeToString(byte[] pArray)
        {
            return StringUtils.NewStringUtf8(Encode(pArray));
        }

        /// <summary>
        ///     Encodes a byte[] containing binary data, into a byte[] containing characters in the Base64 alphabet.
        /// </summary>
        /// <param name="pArray">
        ///     a byte array containing binary data
        /// </param>
        /// <returns> A byte array containing only Base64 character data </returns>
        public virtual byte[] Encode(byte[] pArray)
        {
            Reset();
            if ((pArray == null) || (pArray.Length == 0))
                return pArray;
            var len = GetEncodeLength(pArray, _lineLength, _lineSeparator);
            var buf = new byte[(int) len];
            SetInitialBuffer(buf, 0, buf.Length);
            Encode(pArray, 0, pArray.Length);
            Encode(pArray, 0, -1); // Notify encoder of EOF.
            // Encoder might have resized, even though it was unnecessary.
            if (_buffer != buf)
                ReadResults(buf, 0, buf.Length);
            // In Uri-SAFE mode we skip the padding characters, so sometimes our
            // final length is a bit smaller.
            if (UrlSafe && (_pos < buf.Length))
            {
                var smallerBuf = new byte[_pos];
                Array.Copy(buf, 0, smallerBuf, 0, _pos);
                buf = smallerBuf;
            }
            return buf;
        }

        /// <summary>
        ///     Pre-calculates the amount of space needed to base64-encode the supplied array.
        /// </summary>
        /// <param name="pArray"> byte[] array which will later be encoded </param>
        /// <param name="chunkSize">
        ///     line-length of the output (<= 0 means no chunking) between each
        ///     chunkSeparator (e.g. CRLF). </param>
        /// <param name="chunkSeparator">
        ///     the sequence of bytes used to separate chunks of output (e.g. CRLF).
        /// </param>
        /// <returns>
        ///     amount of space needed to encoded the supplied array.  Returns
        ///     a long since a max-len array will require Integer.MAX_VALUE + 33%.
        /// </returns>
        private static long GetEncodeLength(byte[] pArray, int chunkSize, byte[] chunkSeparator)
        {
            // base64 always encodes to multiples of 4.
            chunkSize = chunkSize/4*4;

            long len = pArray.Length*4/3;
            var mod = len%4;
            if (mod != 0)
                len += 4 - mod;
            if (chunkSize > 0)
            {
                var lenChunksPerfectly = len%chunkSize == 0;
                len += len/chunkSize*chunkSeparator.Length;
                if (!lenChunksPerfectly)
                    len += chunkSeparator.Length;
            }
            return len;
        }

        // Implementation of integer encoding used for crypto
        /// <summary>
        ///     Decodes a byte64-encoded integer according to crypto standards such as W3C's XML-Signature
        /// </summary>
        /// <param name="pArray">
        ///     a byte array containing base64 character data
        /// </param>
        /// <returns>
        ///     A BigInteger
        ///     @since 1.4
        /// </returns>
        public static long DecodeInteger(byte[] pArray)
        {
            var y = (byte[]) (object) pArray;
            var txt = Encoding.Default.GetString(y);
            return long.Parse(txt);
        }


        public static byte[] Long2Bytes(long num)
        {
            var b = new byte[8];
            for (var i = 0; i < 8; i++)
                b[i] = (byte) (num >> (56 - i*8));
            return b;
        }

        /// <summary>
        ///     Returns a byte-array representation of a <code>BigInteger</code> without sign bit.
        /// </summary>
        /// <param name="bigInt">
        ///     <code>BigInteger</code> to be converted
        /// </param>
        /// <returns> a byte array representation of the BigInteger parameter </returns>
        internal static byte[] ToIntegerBytes(long bigInt)
        {
            var bitlen = bigInt.ToString().Length;
            // round bitlen
            bitlen = ((bitlen + 7) >> 3) << 3;
            var bigBytes = Long2Bytes(bigInt);

            if ((bitlen%8 != 0) && (bitlen/8 + 1 == bitlen/8))
                return bigBytes;
            // set up params for copying everything but sign bit
            var startSrc = 0;
            var len = bigBytes.Length;

            // if bigInt is exactly byte-aligned, just skip signbit in copy
            if (bitlen%8 == 0)
            {
                startSrc = 1;
                len--;
            }
            var startDst = bitlen/8 - len; // to pad w/ nulls as per spec
            var resizedBytes = new byte[bitlen/8];
            Array.Copy(bigBytes, startSrc, resizedBytes, startDst, len);
            return resizedBytes;
        }

        /// <summary>
        ///     Resets this Base64 object to its initial newly constructed state.
        /// </summary>
        private void Reset()
        {
            _buffer = null;
            _pos = 0;
            _readPos = 0;
            _currentLinePos = 0;
            _modulus = 0;
            _eof = false;
        }
    }
}