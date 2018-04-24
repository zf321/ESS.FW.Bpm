using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Impl.util
{


    /// <summary>
    /// 
    /// </summary>
    public sealed class StringUtil
    {

        /// <summary>
        /// Checks whether a <seealso cref="String"/> seams to be an expression or not
        /// 
        /// Note: In most cases you should check for composite expressions. See
        /// <seealso cref="#isCompositeExpression(String, ExpressionManager)"/> for more information.
        /// </summary>
        /// <param name="text"> the text to check </param>
        /// <returns> true if the text seams to be an expression false otherwise </returns>
        public static bool IsExpression(string text)
        {
            text = text.Trim();
            return text.StartsWith("${", StringComparison.Ordinal) || text.StartsWith("#{", StringComparison.Ordinal);
        }

        /// <summary>
        /// Checks whether a <seealso cref="String"/> seams to be a composite expression or not. In contrast to an eval expression
        /// is the composite expression also allowed to consist of a combination of literal and eval expressions, e.g.,
        /// "Welcome ${customer.name} to our site".
        /// 
        /// Note: If you just want to allow eval expression, then the expression must always start with "#{" or "${".
        /// Use <seealso cref="#isExpression(String)"/> to conduct these kind of checks.
        /// 
        /// </summary>
        public static bool IsCompositeExpression(string text, ExpressionManager expressionManager)
        {
            return !expressionManager.CreateExpression(text).LiteralText;
        }

        public static string[] Split(string text, string regex)
        {
            if (string.ReferenceEquals(text, null))
            {
                return null;
            }
            else if (string.ReferenceEquals(regex, null))
            {
                return new string[] {text};
            }
            else
            {
                string[] result = text.Split(regex, true);
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = result[i].Trim();
                }
                return result;
            }
        }

        public static bool HasAnySuffix(string text, string[] suffixes)
        {
            foreach (string suffix in suffixes)
            {
                if (text.EndsWith(suffix, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// converts a byte array into a string using the current process engines default charset as
        /// returned by <seealso cref="ProcessEngineConfigurationImpl#getDefaultCharset()"/>
        /// </summary>
        /// <param name="bytes"> the byte array </param>
        /// <returns> a string representing the bytes </returns>
        public static string FromBytes(byte[] bytes)
        {
            EnsureUtil.EnsureActiveCommandContext("StringUtil.fromBytes");
            ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
            return FromBytes(bytes, processEngineConfiguration.ProcessEngine);
        }

        /// <summary>
        /// converts a byte array into a string using the provided process engine's default charset as
        /// returned by <seealso cref="ProcessEngineConfigurationImpl#getDefaultCharset()"/>
        /// </summary>
        /// <param name="bytes"> the byte array </param>
        /// <param name="processEngine"> the process engine </param>
        /// <returns> a string representing the bytes </returns>
        public static string FromBytes(byte[] bytes, IProcessEngine processEngine)
        {
            ProcessEngineConfigurationImpl processEngineConfiguration =
                (ProcessEngineConfigurationImpl) ((ProcessEngineImpl) processEngine).ProcessEngineConfiguration;
            Encoding charset = processEngineConfiguration.DefaultCharset;
            return ESS.FW.Bpm.Engine.Common.StringHelperClass.NewString(bytes, charset.EncodingName);
        }

        public static StreamReader ReaderFromBytes(byte[] bytes)
        {
            EnsureUtil.EnsureActiveCommandContext("StringUtil.readerFromBytes");
            ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
            System.IO.MemoryStream inputStream = new System.IO.MemoryStream(bytes);

            return new System.IO.StreamReader(inputStream, processEngineConfiguration.DefaultCharset);
        }

        public static StreamWriter WriterForStream(System.IO.Stream outStream)
        {
            EnsureUtil.EnsureActiveCommandContext("StringUtil.readerFromBytes");
            ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;

            return new System.IO.StreamWriter(outStream, processEngineConfiguration.DefaultCharset);
        }


        /// <summary>
        /// Gets the bytes from a string using the current process engine's default charset
        /// </summary>
        /// <param name="string"> the string to get the bytes form </param>
        /// <returns> the byte array </returns>
        public static byte[] ToByteArray(string @string)
        {
            EnsureUtil.EnsureActiveCommandContext("StringUtil.toByteArray");
            ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
            return ToByteArray(@string, processEngineConfiguration.ProcessEngine);
        }

        /// <summary>
        /// Gets the bytes from a string using the provided process engine's default charset
        /// </summary>
        /// <param name="string"> the string to get the bytes form </param>
        /// <param name="processEngine"> the process engine to use </param>
        /// <returns> the byte array </returns>
        public static byte[] ToByteArray(string @string, IProcessEngine processEngine)
        {
            ProcessEngineConfigurationImpl processEngineConfiguration =
                (ProcessEngineConfigurationImpl) ((ProcessEngineImpl) processEngine).ProcessEngineConfiguration;
            Encoding charset = processEngineConfiguration.DefaultCharset;
            return @string.GetBytes(charset);
        }

        //public static string JoinDbEntityIds<T1>(ICollection<T1> dbEntities)
        //    where T1 : IDbEntity
        //{
        //    return Join(new StringIteratorAnonymousInnerClass(dbEntities.GetEnumerator()));
        //}

        //private class StringIteratorAnonymousInnerClass : StringIterator<IDbEntity>
        //{
        //    public StringIteratorAnonymousInnerClass(UnknownType iterator) : base(iterator)
        //    {
        //    }

        //    public virtual string Next()
        //    {
        //        return Iterator.next().Id;
        //    }
        //}

        //public static string JoinProcessElementInstanceIds<T1>(ICollection<T1> processElementInstances)
        //    where T1 : IProcessElementInstance
        //{
        //    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //    //ORIGINAL LINE: final java.util.Iterator<? extends org.camunda.bpm.engine.runtime.ProcessElementInstance> iterator = processElementInstances.iterator();
        //    //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
        //    IEnumerator<IProcessElementInstance> iterator = processElementInstances.GetEnumerator();
        //    return Join(new StringIteratorAnonymousInnerClass2(iterator));
        //}

        //private class StringIteratorAnonymousInnerClass2 : StringIterator<IProcessElementInstance>
        //{
        //    //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
        //    //ORIGINAL LINE: private IEnumerator<JavaToDotNetGenericWildcard extends org.camunda.bpm.engine.runtime.ProcessElementInstance> iterator;
        //    private new IEnumerator<IProcessElementInstance> _iterator;

        //    public StringIteratorAnonymousInnerClass2<T1>
        //    (
        //    IEnumerator<T1> Iterator
        //    )
        //    where T1
        //    :
        //    IProcessElementInstance
        //    : base(
        //    iterator
        //    )
        //    {
        //        this.Iterator = Iterator;
        //    }

        //    public virtual string Next()
        //    {
        //        //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
        //        return Iterator.next().Id;
        //    }
        //}

        public static string Join(IEnumerator<string> iterator)
        {
            StringBuilder builder = new StringBuilder();

            while (iterator.MoveNext())
            {
                builder.Append(iterator.Current);
                //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
                if (iterator.MoveNext())
                {
                    builder.Append(", ");
                }
            }

            return builder.ToString();
        }

    //    public abstract class StringIterator<T> : IEnumerator<string>
    //    {

    //        //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
    //        //ORIGINAL LINE: protected java.util.Iterator<? extends T> iterator;
    //        protected internal IEnumerator<T> Iterator;

    //        public StringIterator
    //        (
    //        IEnumerator<T> Iterator
    //        )
    //        where T1
    //        :

    //        T
    //        {
    //            this.Iterator = Iterator;
    //        }

    //        public virtual bool HasNext()
    //        {
    //            //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
    //            return Iterator.hasNext();
    //        }

    //        public virtual void Remove()
    //        {
    //            //JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
    //            Iterator.remove();
    //        }
    //    }

    }


//-------------------------------------------------------------------------------------------
//	Copyright © 2007 - 2017 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class is used to convert some aspects of the Java String class.
//-------------------------------------------------------------------------------------------
    internal static class StringHelperClass
    {
        //----------------------------------------------------------------------------------
        //	This method replaces the Java String.substring method when 'start' is a
        //	method call or calculated value to ensure that 'start' is obtained just once.
        //----------------------------------------------------------------------------------
        internal static string SubstringSpecial(this string self, int start, int end)
        {
            return self.Substring(start, end - start);
        }

        //------------------------------------------------------------------------------------
        //	This method is used to replace calls to the 2-arg Java String.startsWith method.
        //------------------------------------------------------------------------------------
        internal static bool StartsWith(this string self, string prefix, int toffset)
        {
            return self.IndexOf(prefix, toffset, System.StringComparison.Ordinal) == toffset;
        }

        //------------------------------------------------------------------------------
        //	This method is used to replace most calls to the Java String.split method.
        //------------------------------------------------------------------------------
        internal static string[] Split(this string self, string regexDelimiter, bool trimTrailingEmptyStrings)
        {
            string[] splitArray = System.Text.RegularExpressions.Regex.Split(self, regexDelimiter);

            if (trimTrailingEmptyStrings)
            {
                if (splitArray.Length > 1)
                {
                    for (int i = splitArray.Length; i > 0; i--)
                    {
                        if (splitArray[i - 1].Length > 0)
                        {
                            if (i < splitArray.Length)
                                System.Array.Resize(ref splitArray, i);

                            break;
                        }
                    }
                }
            }

            return splitArray;
        }

        //-----------------------------------------------------------------------------
        //	These methods are used to replace calls to some Java String constructors.
        //-----------------------------------------------------------------------------
        internal static string NewString(byte[] bytes)
        {
            return NewString(bytes, 0, bytes.Length);
        }

        internal static string NewString(byte[] bytes, int index, int count)
        {
            return System.Text.Encoding.UTF8.GetString((byte[]) (object) bytes, index, count);
        }

        internal static string NewString(byte[] bytes, string encoding)
        {
            return NewString(bytes, 0, bytes.Length, encoding);
        }

        internal static string NewString(byte[] bytes, int index, int count, string encoding)
        {
            return System.Text.Encoding.GetEncoding(encoding).GetString((byte[]) (object) bytes, index, count);
        }

        //--------------------------------------------------------------------------------
        //	These methods are used to replace calls to the Java String.getBytes methods.
        //--------------------------------------------------------------------------------
        internal static byte[] GetBytes(this string self)
        {
            return GetSBytesForEncoding(System.Text.Encoding.UTF8, self);
        }

        internal static byte[] GetBytes(this string self, System.Text.Encoding encoding)
        {
            return GetSBytesForEncoding(encoding, self);
        }

        internal static byte[] GetBytes(this string self, string encoding)
        {
            return GetSBytesForEncoding(System.Text.Encoding.GetEncoding(encoding), self);
        }

        private static byte[] GetSBytesForEncoding(System.Text.Encoding encoding, string s)
        {
            byte[] sbytes = new byte[encoding.GetByteCount(s)];
            encoding.GetBytes(s, 0, s.Length, (byte[]) (object) sbytes, 0);
            return sbytes;
        }
    }
}