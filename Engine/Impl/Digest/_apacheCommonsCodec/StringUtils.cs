using System;
using System.Text;
using ESS.FW.Bpm.Engine.Common;

namespace ESS.FW.Bpm.Engine.Impl.Digest._apacheCommonsCodec
{
    /// <summary>
    ///     Converts String to and from bytes using the encodings required by the Java specification. These encodings are
    ///     specified in
    ///     <a
    ///         href="http://java.sun.com/j2se/1.4.2/docs/api/java/nio/charset/Encoding.html">
    ///         Standard charsets
    ///     </a>
    /// </summary>
    /// <seealso cref=
    /// <a href="http://commons.apache.org/proper/commons-codec/apidocs/org/apache/commons/codec/CharEncoding.html">CharEncoding</a>
    /// </seealso>
    /// <seealso cref=
    /// <a href="http://java.sun.com/j2se/1.4.2/docs/api/java/nio/charset/Encoding.html">Standard charsets</a>
    /// 
    /// <a href="mailto:ggregory@seagullsw.com">Gary Gregory</a>
    /// @version $Id: StringUtils.java 801391 2009-08-05 19:55:54Z ggregory $
    /// @since 1.4
    /// </seealso>
    public class StringUtils
    {
        public const string Utf8 = "UTF-8";

        /// <summary>
        ///     Constructs a new <code>String</code> by decoding the specified array of bytes using the given charset.
        ///     <para>
        ///         This method catches <seealso cref="UnsupportedEncodingException" /> and re-throws it as
        ///         <seealso cref="IllegalStateException" />, which
        ///         should never happen for a required charset name. Use this method when the encoding is required to be in the
        ///         JRE.
        ///     </para>
        /// </summary>
        /// <param name="bytes">
        ///     The bytes to be decoded into characters
        /// </param>
        /// <param name="charsetName">
        ///     The name of a required <seealso cref="Encoding" />
        /// </param>
        /// <returns> A new <code>String</code> decoded from the specified array of bytes using the given charset. </returns>
        /// <exception cref="IllegalStateException">
        ///     Thrown when a <seealso cref="UnsupportedEncodingException" /> is caught, which should never happen for a
        ///     required charset name.
        /// </exception>
        /// <seealso cref=
        /// <a href="http://commons.apache.org/proper/commons-codec/apidocs/org/apache/commons/codec/CharEncoding.html">CharEncoding</a>
        /// </seealso>
        /// <seealso cref= String# String( byte[], String
        /// )
        /// </seealso>
        public static string NewString(byte[] bytes, string charsetName)
        {
            if (bytes == null)
                return null;
            try
            {
                return StringHelperClass.NewString(bytes, charsetName);
            }
            catch (System.Exception e)
            {
                throw NewIllegalStateException(charsetName, e);
            }
        }

        /// <summary>
        ///     Constructs a new <code>String</code> by decoding the specified array of bytes using the UTF-8 charset.
        /// </summary>
        /// <param name="bytes">
        ///     The bytes to be decoded into characters
        /// </param>
        /// <returns> A new <code>String</code> decoded from the specified array of bytes using the given charset. </returns>
        /// <exception cref="IllegalStateException">
        ///     Thrown when a <seealso cref="UnsupportedEncodingException" /> is caught, which should never happen since the
        ///     charset is required.
        /// </exception>
        public static string NewStringUtf8(byte[] bytes)
        {
            return NewString(bytes, Utf8);
        }

        /// <summary>
        ///     Encodes the given string into a sequence of bytes using the UTF-8 charset, storing the result into a new byte
        ///     array.
        /// </summary>
        /// <param name="string">
        ///     the String to encode
        /// </param>
        /// <returns> encoded bytes </returns>
        /// <exception cref="IllegalStateException">
        ///     Thrown when the charset is missing, which should be never according the the Java specification.
        /// </exception>
        /// <seealso cref=
        /// <a href="http://java.sun.com/j2se/1.4.2/docs/api/java/nio/charset/Encoding.html">Standard charsets</a>
        /// </seealso>
        /// <seealso cref= # getBytesUnchecked( String, String
        /// )
        /// </seealso>
        public static byte[] GetBytesUtf8(string @string)
        {
            return GetBytesUnchecked(@string, Utf8);
        }

        /// <summary>
        ///     Encodes the given string into a sequence of bytes using the named charset, storing the result into a new byte
        ///     array.
        ///     <para>
        ///         This method catches <seealso cref="UnsupportedEncodingException" /> and rethrows it as
        ///         <seealso cref="IllegalStateException" />, which
        ///         should never happen for a required charset name. Use this method when the encoding is required to be in the
        ///         JRE.
        ///     </para>
        /// </summary>
        /// <param name="string">
        ///     the String to encode
        /// </param>
        /// <param name="charsetName">
        ///     The name of a required <seealso cref="Encoding" />
        /// </param>
        /// <returns> encoded bytes </returns>
        /// <exception cref="IllegalStateException">
        ///     Thrown when a <seealso cref="UnsupportedEncodingException" /> is caught, which should never happen for a
        ///     required charset name.
        /// </exception>
        /// <seealso cref=
        /// <a href="http://commons.apache.org/proper/commons-codec/apidocs/org/apache/commons/codec/CharEncoding.html">CharEncoding</a>
        /// </seealso>
        /// <seealso cref= String# getBytes( String
        /// )
        /// </seealso>
        public static byte[] GetBytesUnchecked(string @string, string charsetName)
        {
            if (ReferenceEquals(@string, null))
                return null;
            try
            {
                return @string.GetBytes(charsetName);
            }
            catch (System.Exception e)
            {
                //throw new IllegalStateException(charsetName, e);
                throw e;
            }
        }

        private static InvalidOperationException NewIllegalStateException(string charsetName,
            System.Exception e)
        {
            return new InvalidOperationException(charsetName + ": " + e);
        }
    }
}

