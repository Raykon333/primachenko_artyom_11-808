using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace GachiMail.Utilities.FileLogger
{
    public class FileLogger : ILogger
    {
        private string filePath;
        private static object _lock = new object();
        public FileLogger(string path)
        {
            filePath = path;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel level)
        {
            return true;
        }

        public void Log<TState>(LogLevel level, 
            EventId id, 
            TState state, 
            Exception exc, 
            Func<TState, Exception, string>formatter)
        {
            if(formatter != null)
            {
                lock (_lock)
                {
                    File.AppendAllText(filePath, formatter(state, exc) + Environment.NewLine);
                    Console.WriteLine(formatter(state, exc));
                }
            }
        }
    }
}
