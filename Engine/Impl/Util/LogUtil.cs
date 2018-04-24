using System;
using System.Collections.Generic;
using System.Text;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Util
{
    /// <summary>
    ///      
    /// </summary>
    /// @deprecated since 7.4, use slf4j
    [Obsolete("since 7.4, use slf4j")]
    public class LogUtil
    {
        public enum ThreadLogMode
        {
            None,
            Indent,
            PrintId
        }

        public const string Format = "HH:mm:ss,SSS";

        private static readonly string LineSeparator = "--";
        private static IDictionary<int?, string> _threadIndents = new Dictionary<int?, string>();
        private static ThreadLogMode _threadLogMode = ThreadLogMode.None;

        public static ThreadLogMode GetThreadLogMode()
        {
            return _threadLogMode;
        }

        public static ThreadLogMode SetThreadLogMode(ThreadLogMode threadLogMode)
        {
            var old = LogUtil._threadLogMode;
            LogUtil._threadLogMode = threadLogMode;
            return old;
        }

        public static void ReadJavaUtilLoggingConfigFromClasspath()
        {
            var inputStream = ReflectUtil.GetResourceAsStream("logging.properties");
            try
            {
                if (inputStream != null)
                {
                    //LogManager.LogManager.readConfiguration(inputStream);

                    //string redirectCommons = LogManager.LogManager.getProperty("redirect.commons.logging");
                    //if ((!ReferenceEquals(redirectCommons, null)) &&
                    //    (!redirectCommons.Equals("false", StringComparison.CurrentCultureIgnoreCase)))
                    //{
                    //    System.setProperty("org.apache.commons.logging.Log",
                    //        "org.apache.commons.logging.impl.Jdk14Logger");
                    //}
                }
            }
            catch (System.Exception e)
            {
                throw new PvmException("couldn't initialize logging properly", e);
            }
            finally
            {
                IoUtil.CloseSilently(inputStream);
            }
        }

        public static void ResetThreadIndents()
        {
            _threadIndents = new Dictionary<int?, string>();
        }

        public class LogFormatter //: Formatter
        {
            //public virtual string format(LogRecord record)
            //{
            //    var line = new StringBuilder();
            //    line.Append(dateFormat.format(DateTime.Now));
            //    if (Level.FINE.Equals(record.Level))
            //    {
            //        line.Append(" FIN ");
            //    }
            //    else if (Level.FINEST.Equals(record.Level))
            //    {
            //        line.Append(" FST ");
            //    }
            //    else if (Level.INFO.Equals(record.Level))
            //    {
            //        line.Append(" INF ");
            //    }
            //    else if (Level.SEVERE.Equals(record.Level))
            //    {
            //        line.Append(" SEV ");
            //    }
            //    else if (Level.WARNING.Equals(record.Level))
            //    {
            //        line.Append(" WRN ");
            //    }
            //    else if (Level.FINER.Equals(record.Level))
            //    {
            //        line.Append(" FNR ");
            //    }
            //    else if (Level.CONFIG.Equals(record.Level))
            //    {
            //        line.Append(" CFG ");
            //    }

            //    int threadId = record.ThreadID;
            //    var threadIndent = getThreadIndent(threadId);

            //    line.Append(threadIndent);
            //    line.Append(" | ");
            //    line.Append(record.Message);

            //    if (record.Thrown != null)
            //    {
            //        line.Append(LINE_SEPARATOR);

            //        StringWriter stringWriter = new StringWriter();
            //        PrintWriter printWriter = new PrintWriter(stringWriter);
            //        record.Thrown.printStackTrace(printWriter);
            //        line.Append(stringWriter.ToString());
            //    }

            //    line.Append("  [");
            //    line.Append(record.LoggerName);
            //    line.Append("]");

            //    line.Append(LINE_SEPARATOR);

            //    return line.ToString();
            //}

            protected internal static string GetThreadIndent(int threadId)
            {
                int? threadIdInteger = threadId;
                if (_threadLogMode == ThreadLogMode.None)
                    return "";
                if (_threadLogMode == ThreadLogMode.PrintId)
                    return "" + threadId;
                var threadIndent = _threadIndents[threadIdInteger];
                if (ReferenceEquals(threadIndent, null))
                {
                    var stringBuilder = new StringBuilder();
                    for (var i = 0; i < _threadIndents.Count; i++)
                        stringBuilder.Append("  ");
                    threadIndent = stringBuilder.ToString();
                    _threadIndents[threadIdInteger] = threadIndent;
                }
                return threadIndent;
            }
        }
    }
}