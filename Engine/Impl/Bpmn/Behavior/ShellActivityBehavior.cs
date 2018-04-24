using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Model.Bpmn.builder;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    public class ShellActivityBehavior : AbstractBpmnActivityBehavior
    {
        protected internal new static readonly BpmnBehaviorLogger Log = ProcessEngineLogger.BpmnBehaviorLogger;
        protected internal IExpression Arg1;
        internal string Arg1Str;
        protected internal IExpression Arg2;
        internal string Arg2Str;
        protected internal IExpression Arg3;
        internal string Arg3Str;
        protected internal IExpression Arg4;
        internal string Arg4Str;
        protected internal IExpression Arg5;
        internal string Arg5Str;
        protected internal IExpression CleanEnv;
        internal bool? CleanEnvBoolan;

        protected internal IExpression Command;

        internal string CommandStr;
        protected internal IExpression Directory;
        internal string DirectoryStr;
        protected internal IExpression ErrorCodeVariable;
        internal string ErrorCodeVariableStr;
        protected internal IExpression OutputVariable;
        protected internal IExpression RedirectError;
        internal bool? RedirectErrorFlag;
        internal string ResultVariableStr;
        protected internal IExpression Wait;
        internal bool? WaitFlag;
        internal string WaitStr;

        private void ReadFields(IActivityExecution execution)
        {
            CommandStr = GetStringFromField(Command, execution);
            Arg1Str = GetStringFromField(Arg1, execution);
            Arg2Str = GetStringFromField(Arg2, execution);
            Arg3Str = GetStringFromField(Arg3, execution);
            Arg4Str = GetStringFromField(Arg4, execution);
            Arg5Str = GetStringFromField(Arg5, execution);
            WaitStr = GetStringFromField(Wait, execution);
            ResultVariableStr = GetStringFromField(OutputVariable, execution);
            ErrorCodeVariableStr = GetStringFromField(ErrorCodeVariable, execution);

            var redirectErrorStr = GetStringFromField(RedirectError, execution);
            var cleanEnvStr = GetStringFromField(CleanEnv, execution);

            WaitFlag = ReferenceEquals(WaitStr, null) || WaitStr.Equals("true");
            RedirectErrorFlag = !ReferenceEquals(redirectErrorStr, null) && redirectErrorStr.Equals("true");
            CleanEnvBoolan = !ReferenceEquals(cleanEnvStr, null) && cleanEnvStr.Equals("true");
            DirectoryStr = GetStringFromField(Directory, execution);
        }

        public override void Execute(IActivityExecution execution)
        {
            ReadFields(execution);

            IList<string> argList = new List<string>();
            argList.Add(CommandStr);

            if (!ReferenceEquals(Arg1Str, null))
                argList.Add(Arg1Str);
            if (!ReferenceEquals(Arg2Str, null))
                argList.Add(Arg2Str);
            if (!ReferenceEquals(Arg3Str, null))
                argList.Add(Arg3Str);
            if (!ReferenceEquals(Arg4Str, null))
                argList.Add(Arg4Str);
            if (!ReferenceEquals(Arg5Str, null))
                argList.Add(Arg5Str);

            //ProcessBuilder processBuilder = new ProcessBuilder(argList);
            throw new NotImplementedException();
            //try
            //{
            //    processBuilder.redirectErrorStream(redirectErrorFlag);
            //    if (cleanEnvBoolan.Value)
            //    {
            //        IDictionary<string, string> env = processBuilder.environment();
            //        env.Clear();
            //    }
            //    if (!ReferenceEquals(directoryStr, null) && directoryStr.Length > 0)
            //    {
            //        processBuilder.directory(new FileInfo(directoryStr));
            //    }

            //    System.Diagnostics.Process process = processBuilder.Start();

            //    if (waitFlag.Value)
            //    {
            //        int errorCode = process.waitFor();

            //        if (!ReferenceEquals(resultVariableStr, null))
            //        {
            //            var result = convertStreamToStr(process.InputStream);
            //            execution.setVariable(resultVariableStr, result);
            //        }

            //        if (!ReferenceEquals(errorCodeVariableStr, null))
            //        {
            //            execution.setVariable(errorCodeVariableStr, Convert.ToString(errorCode));
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    throw LOG.shellExecutionException(e);
            //}

            //Leave(execution);
        }
        
        public static string ConvertStreamToStr(Stream @is)
        {
            if (@is != null)
            {
                var writer = new StringWriter();

                var buffer = new char[1024];
                try
                {
                    var reader = new StreamReader(@is, Encoding.UTF8);
                    int n;
                    while ((n = reader.Read()) != -1)
                        writer.Write(buffer, 0, n);
                }
                finally
                {
                    @is.Close();
                }
                return writer.ToString();
            }
            return "";
        }

        protected internal virtual string GetStringFromField(IExpression expression, IDelegateExecution execution)
        {
            if (expression != null)
            {
                var value = expression.GetValue(execution);
                if (value != null)
                    return value.ToString();
            }
            return null;
        }
    }
}