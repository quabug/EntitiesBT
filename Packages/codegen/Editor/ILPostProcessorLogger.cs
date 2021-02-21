using System;
using System.Collections.Generic;
using Unity.CompilationPipeline.Common.Diagnostics;

namespace EntitiesBT.CodeGen.Editor
{
    public class ILPostProcessorLogger : ILogger
    {
        public readonly List<DiagnosticMessage> Messages;

        public ILPostProcessorLogger(List<DiagnosticMessage> messages)
        {
            Messages = messages;
        }

        public void Error(string message)
        {
            if (Messages == null) Console.WriteLine("error: " + message);
            else Messages.Add(new DiagnosticMessage {DiagnosticType = DiagnosticType.Error, MessageData = message});
        }

        public void Warning(string message)
        {
            if (Messages == null) Console.WriteLine("warning: " + message);
            else Messages.Add(new DiagnosticMessage {DiagnosticType = DiagnosticType.Warning, MessageData = message});
        }

        public void Info(string message)
        {
            Console.WriteLine("info: " + message);
        }

        public void Debug(string message)
        {
            Console.WriteLine("debug: " + message);
        }
    }
}