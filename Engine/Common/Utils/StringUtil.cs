using System;
using System.Text;

namespace ESS.FW.Bpm.Engine.Common.Utils
{
    /// <summary>
    /// </summary>
    public sealed class StringUtil
    {
        /// <summary>
        ///     Checks whether a <seealso cref="string" /> seams to be an expression or not
        /// </summary>
        /// <param name="text"> the text to check </param>
        /// <returns> true if the text seams to be an expression false otherwise </returns>
        public static bool IsExpression(string text)
        {
            if (ReferenceEquals(text, null))
                return false;
            text = text.Trim();
            return text.StartsWith("${", StringComparison.Ordinal) || text.StartsWith("#{", StringComparison.Ordinal);
        }

        /// <summary>
        ///     Splits a <seealso cref="string" /> by an expression.
        /// </summary>
        /// <param name="text"> the text to split </param>
        /// <param name="regex"> the regex to split by </param>
        /// <returns> the parts of the text or null if text was null </returns>
        public static string[] Split(string text, string regex)
        {
            if (ReferenceEquals(text, null))
                return null;
            if (ReferenceEquals(regex, null))
                return new[] { text };
            var result = text.Split(regex, true);
            for (var i = 0; i < result.Length; i++)
                result[i] = result[i].Trim();
            return result;
        }

        /// <summary>
        ///     Joins a list of Strings to a single one.
        /// </summary>
        /// <param name="delimiter"> the delimiter between the joined parts </param>
        /// <param name="parts"> the parts to join </param>
        /// <returns> the joined String or null if parts was null </returns>
        public static string Join(string delimiter, params string[] parts)
        {
            if (parts == null)
                return null;

            if (ReferenceEquals(delimiter, null))
                delimiter = "";

            var stringBuilder = new StringBuilder();
            for (var i = 0; i < parts.Length; i++)
            {
                if (i > 0)
                    stringBuilder.Append(delimiter);
                stringBuilder.Append(parts[i]);
            }
            return stringBuilder.ToString();
        }

        public static bool HasAnySuffix(string text, string[] suffixes)
        {
            foreach (var item in suffixes)
            {
                if (text.EndsWith(item))
                {
                    return true;
                }
            }
            return false;
        }
    }
}