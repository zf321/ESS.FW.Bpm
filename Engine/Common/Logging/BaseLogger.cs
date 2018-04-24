using System;
using System.Diagnostics;

namespace ESS.FW.Bpm.Engine.Common.Logging
{
    /// <summary>
    ///     Base class for implementing a logger class. A logger class is a class with
    ///     dedicated methods for each log message:
    ///     <pre>
    ///         public class MyLogger extends BaseLogger {
    ///         public static MyLogger LOG = createLogger(MyLogger.class, "MYPROJ", "org.example", "01");
    ///         public void engineStarted(long currentTime) {
    ///         logInfo("100", "My super engine has started at '{}'", currentTime);
    ///         }
    ///         }
    ///     </pre>
    ///     The logger can then be used in the following way:
    ///     <pre>
    ///         LOG.engineStarted(System.currentTimeMilliseconds());
    ///     </pre>
    ///     This will print the following message:
    ///     <pre>
    ///         INFO  org.example - MYPROJ-01100 My super engine has started at '4234234523'
    ///     </pre>
    ///     <h2>Slf4j</h2>
    ///     This class uses slf4j as logging API. The class ensures that log messages and exception
    ///     messages are always formatted using the same template.
    ///     <h2>Log message format</h2>
    ///     The log message format produced by this class is as follows:
    ///     <pre>
    ///         [PROJECT_CODE]-[COMPONENT_ID][MESSAGE_ID] message
    ///     </pre>
    ///     Example:
    ///     <pre>
    ///         MYPROJ-01100 My super engine has started at '4234234523'
    ///     </pre>
    ///     
    ///     
    /// </summary>
    [System.Serializable]
    public abstract class BaseLogger
    {
        /// <summary>
        ///     the component Id of the logger.
        /// </summary>
        public string ComponentId;

        /// <summary>
        ///     the slf4j logger we delegate to
        /// </summary>
        //public ILogger DelegateLogger;

        /// <summary>
        ///     the project code of the logger
        /// </summary>
        public string ProjectCode;

        public BaseLogger()
        {
        }

        /// <returns> true if the logger will log 'DEBUG' messages </returns>
        public virtual bool DebugEnabled
        {
            get { return true; }
        }

        /// <returns> true if the logger will log 'INFO' messages </returns>
        public virtual bool InfoEnabled
        {
            get { return true; }
        }

        /// <returns> true if the logger will log 'WARN' messages </returns>
        public virtual bool WarnEnabled
        {
            get { return true; }
        }

        /// <returns> true if the logger will log 'ERROR' messages </returns>
        public virtual bool ErrorEnabled
        {
            get { return true; }
        }

        /// <summary>
        ///     Creates a new instance of the <seealso cref="BaseLogger Logger" />.
        /// </summary>
        /// <param name="loggerClass"> the type of the logger </param>
        /// <param name="projectCode"> the unique code for a complete project. </param>
        /// <param name="name"> the name of the slf4j logger to use. </param>
        /// <param name="componentId"> the unique id of the component. </param>
        public static T CreateLogger<T>(Type loggerClass, string projectCode, string name, string componentId)
            where T : BaseLogger
        {
            //try
            //{
            var logger = Activator.CreateInstance(loggerClass) as T;
            logger.ProjectCode = projectCode;
            logger.ComponentId = componentId;
            //logger.delegateLogger = LoggerFactory.getLogger(name);

            return logger;

            //}
            //		catch (InstantiationException e)
            //		{
            ////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
            //		  throw new Exception("Unable to instantiate logger '" + loggerClass.FullName + "'", e);

            //		}
            //		catch (AccessViolationException e)
            //		{
            ////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
            //		  throw new Exception("Unable to instantiate logger '" + loggerClass.FullName + "'", e);

            //		}
        }

        /// <summary>
        ///     Logs a 'DEBUG' message
        /// </summary>
        /// <param name="id"> the unique id of this log message </param>
        /// <param name="messageTemplate"> the message template to use </param>
        /// <param name="parameters"> a list of optional parameters </param>
        public virtual void LogDebug(string id, string messageTemplate, params object[] parameters)
        {
            if (DebugEnabled)
            {
                string msg = FormatMessageTemplate(id, messageTemplate);
                Debug.WriteLine(msg, parameters);
            }
        }

        /// <summary>
        ///     Logs an 'INFO' message
        /// </summary>
        /// <param name="id"> the unique id of this log message </param>
        /// <param name="messageTemplate"> the message template to use </param>
        /// <param name="parameters"> a list of optional parameters </param>
        public virtual void LogInfo(string id, string messageTemplate, params object[] parameters)
        {
            if (InfoEnabled)
            {
                string msg = FormatMessageTemplate(id, messageTemplate);
                Debug.WriteLine(msg, parameters);
            }
        }

        /// <summary>
        ///     Logs an 'WARN' message
        /// </summary>
        /// <param name="id"> the unique id of this log message </param>
        /// <param name="messageTemplate"> the message template to use </param>
        /// <param name="parameters"> a list of optional parameters </param>
        public virtual void LogWarn(string id, string messageTemplate, params object[] parameters)
        {
            if (WarnEnabled)
            {
                string msg = FormatMessageTemplate(id, messageTemplate);
                Debug.WriteLine(msg, parameters);
            }
        }

        /// <summary>
        ///     Logs an 'ERROR' message
        /// </summary>
        /// <param name="id"> the unique id of this log message </param>
        /// <param name="messageTemplate"> the message template to use </param>
        /// <param name="parameters"> a list of optional parameters </param>
        public virtual void LogError(string id, string messageTemplate, params object[] parameters)
        {
            if (ErrorEnabled)
            {
                string msg = FormatMessageTemplate(id, messageTemplate);
                Debug.WriteLine(msg, parameters);
            }
        }

        /// <summary>
        ///     Formats a message template
        /// </summary>
        /// <param name="id"> the id of the message </param>
        /// <param name="messageTemplate">
        ///     the message template to use
        /// </param>
        /// <returns> the formatted template </returns>
        public virtual string FormatMessageTemplate(string id, string messageTemplate)
        {
            return ProjectCode + "-" + ComponentId + id + " " + messageTemplate;
        }

        /// <summary>
        ///     Prepares an exception message
        /// </summary>
        /// <param name="id"> the id of the message </param>
        /// <param name="messageTemplate"> the message template to use </param>
        /// <param name="parameters">
        ///     the parameters for the message (optional)
        /// </param>
        /// <returns> the prepared exception message </returns>
        public virtual string ExceptionMessage(string id, string messageTemplate, params object[] parameters)
        {
            var formattedTemplate = FormatMessageTemplate(id, messageTemplate);
            if ((parameters == null) || (parameters.Length == 0))
                return formattedTemplate;
            //return MessageFormatter.arrayFormat(formattedTemplate, parameters).Message;
            //return formattedTemplate;
            return string.Format(formattedTemplate, parameters);
        }
        protected virtual string GetAllMessageFromException(System.Exception e)
        {
            string msg = string.Empty;
            return GetMsgFromExc(e, msg);
        }
        private string GetMsgFromExc(System.Exception e, string msg)
        {
            msg += e == null ? "" : e.Message;
            if (e.InnerException != null)
            {
                msg += GetMsgFromExc(e.InnerException, msg);
            }
            return msg;
        }
    }

}