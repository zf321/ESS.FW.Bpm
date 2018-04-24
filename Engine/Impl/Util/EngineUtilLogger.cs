using System;
using System.IO;
using System.Threading;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Impl.Util.IO;

namespace ESS.FW.Bpm.Engine.Impl.Util
{
    /// <summary>
    ///     
    /// </summary>
    public class EngineUtilLogger : ProcessEngineLogger
    {
        public virtual ProcessEngineException MalformedUrlException(string url, System.Exception cause)
        {
            return new ProcessEngineException(ExceptionMessage("001", "The Uri '{0}' is malformed", url), cause);
        }

        public virtual ProcessEngineException MultipleSourcesException(IStreamSource source1, IStreamSource source2)
        {
            return
                new ProcessEngineException(ExceptionMessage("002",
                    "Multiple sources detected, which is invalid. Source 1: '{0}', Source 2: {1}", source1, source2));
        }

        public virtual ProcessEngineException ParsingFailureException(string name, System.Exception cause)
        {
            return new ProcessEngineException(ExceptionMessage("003", "Could not parse '{0}'.", name), cause);
        }

        public virtual void LogParseWarnings(string formattedMessage)
        {
            LogWarn("004", "Warnings during parsing: {0}", formattedMessage);
        }

        public virtual ProcessEngineException ExceptionDuringParsing(string @string)
        {
            return
                new ProcessEngineException(ExceptionMessage("005", "Could not parse BPMN process. Errors: {0}", @string));
        }


        public virtual void UnableToSetSchemaResource(System.Exception cause)
        {
            LogWarn("006", "Setting schema resource failed because of: '{0}'", cause.Message, cause);
        }

        public virtual ProcessEngineException InvalidBitNumber(int bitNumber)
        {
            return
                new ProcessEngineException(ExceptionMessage("007", "Invalid bit {0}. Only 8 bits are supported.",
                    bitNumber));
        }

        public virtual ProcessEngineException ExceptionWhileInstantiatingClass(string className, System.Exception e)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("008", "Exception while instantiating class '{0}': {1}", className, e.Message), e);
        }

        public virtual ProcessEngineException ExceptionWhileApplyingFieldDeclatation(string declName, string className,
            System.Exception e)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("009", "Exception while applying field declaration '{0}' on class '{1}': {2}",
                        declName, className, e.Message), e);
        }

//        public virtual ProcessEngineException incompatibleTypeForFieldDeclaration(FieldDeclaration declaration,
//            object target, Field field)
//        {
////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
//            return
//                new ProcessEngineException(ExceptionMessage("010",
//                    "Incompatible type set on field declaration '{}' for class '{}'. Declared value has type '{}', while expecting '{}'",
//                    declaration.Name, target.GetType().FullName, declaration.Value.GetType().FullName, field.Type.Name));
//        }

        public virtual ProcessEngineException ExceptionWhileReadingStream(string inputStreamName, System.Exception e)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("011", "Exception while reading {0} as input stream: {1}", inputStreamName, e.Message),
                    e);
        }

        public virtual ProcessEngineException ExceptionWhileReadingFile(string filePath, System.Exception e)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("012", "Exception while reading file {0}: {1}", filePath, e.Message), e);
        }

        public virtual ProcessEngineException ExceptionWhileGettingFile(string filePath, System.Exception e)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("013", "Exception while getting file {0}: {1}", filePath, e.Message), e);
        }

        public virtual ProcessEngineException ExceptionWhileWritingToFile(string filePath, System.Exception e)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("014", "Exception while writing to file {0}: {1}", filePath, e.Message), e);
        }

        public virtual void DebugCloseException(IOException ignore)
        {
            LogDebug("015", "Ignored exception on resource close", ignore);
        }

        public virtual void DebugClassLoading(string className, string classLoaderDescription)
        {
            //logDebug("016", "Attempting to load class '{}' with {}: {}", className, classLoaderDescription, classLoader);
        }

        public virtual ClassLoadingException ClassLoadingException(string className, System.Exception throwable)
        {
            return
                new ClassLoadingException(
                    ExceptionMessage("017", "Cannot load class '{0}': {1}", className, throwable.Message), className,
                    throwable);
        }

        //public virtual ProcessEngineException cannotConvertUrlToUri(Uri url, URISyntaxException e)
        //{
        //    return
        //        new ProcessEngineException(
        //            ExceptionMessage("018", "Cannot convert Uri[{}] to URI: {}", url, e.Message), e);
        //}

        public virtual ProcessEngineException ExceptionWhileInvokingMethod(string methodName, object target, System.Exception e)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("019", "Exception while invoking method '{0}' on object of type '{1}': {2}'",
                        methodName, target, e.Message), e);
        }

        //public virtual ProcessEngineException unableToAccessField(Field field, string name)
        //{
        //    return
        //        new ProcessEngineException(ExceptionMessage("020",
        //            "Unable to access field {} on class {}, access protected", field, name));
        //}

        public virtual ProcessEngineException UnableToAccessMethod(string methodName, string name)
        {
            return
                new ProcessEngineException(ExceptionMessage("021",
                    "Unable to access method {0} on class {1}, access protected", methodName, name));
        }

//        public virtual ProcessEngineException exceptionWhileSettingField(Field field, object @object, object value,
//            Exception e)
//        {
////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
//            return
//                new ProcessEngineException(
//                    ExceptionMessage("022",
//                        "Exception while setting value '{}' to field '{}' on object of type '{}': {}", value, field,
//                        @object.GetType().FullName, e.Message), e);
//        }

        public virtual ProcessEngineException AmbiguousSetterMethod(string setterName, string name)
        {
            return
                new ProcessEngineException(ExceptionMessage("023",
                    "Ambiguous setter: more than one method named {} on class {}, with different parameter types.",
                    setterName, name));
        }

        public virtual NotFoundException CannotFindResource(string resourcePath)
        {
            return new NotFoundException(ExceptionMessage("024", "Unable to find resource at path {0}", resourcePath));
        }

        public virtual InvalidOperationException NotInsideCommandContext(string operation)
        {
            return
                new InvalidOperationException(ExceptionMessage("025",
                    "Operation {0} requires active command context. No command context active on thread {1}.", operation,
                    Thread.CurrentThread));
        }

        public virtual ProcessEngineException ExceptionWhileParsingCronExpresison(string duedateDescription, System.Exception e)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("026", "Exception while parsing cron expression '{0}': {1}", duedateDescription,
                        e.Message), e);
        }

        public virtual ProcessEngineException ExceptionWhileResolvingDuedate(string duedate, System.Exception e)
        {
            return
                new ProcessEngineException(
                    ExceptionMessage("027", "Exception while resolving duedate '{0}': {1}", duedate, e.Message), e);
        }

        public virtual System.Exception CannotParseDuration(string expressions)
        {
            return new ProcessEngineException(ExceptionMessage("028", "Cannot parse duration '{0}'.", expressions));
        }
    }
}