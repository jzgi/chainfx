using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Creatbone.Service
{
    public sealed class Logger : ILoggerProvider, ILogger
    {
        // opened writer on the log file
        readonly StreamWriter logWriter;

        internal Logger(string file)
        {
            // init the file-based logger
            FileStream stream = new FileStream(file, FileMode.Append, FileAccess.Write);
            logWriter = new StreamWriter(stream, Encoding.UTF8, 4096, false)
            {
                AutoFlush = true
            };
        }

        public int Level { get; internal set; } = 3;

        //
        // LOGGING

        // subworks are already there
        public ILogger CreateLogger(string name)
        {
            return this;
        }

        public IDisposable BeginScope<T>(T state)
        {
            return this;
        }

        public bool IsEnabled(LogLevel level)
        {
            return (int) level >= this.Level;
        }

        static readonly string[] LVL =
        {
            "TRC: ", "DBG: ", "INF: ", "WAR: ", "ERR: ", "CRL: ", "NON: "
        };

        public void Log<T>(LogLevel level, EventId eid, T state, Exception except, Func<T, Exception, string> formatter)
        {
            if (!IsEnabled(level))
            {
                return;
            }

            logWriter.Write(LVL[(int) level]);

            if (eid.Id != 0)
            {
                logWriter.Write("{");
                logWriter.Write(eid.Id);
                logWriter.Write("} ");
            }

            if (formatter != null) // custom format
            {
                var msg = formatter(state, except);
                logWriter.WriteLine(msg);
            }
            else // fixed format
            {
                logWriter.WriteLine(state.ToString());
                if (except != null)
                {
                    logWriter.WriteLine(except.StackTrace);
                }
            }
        }

        // end of a logger scope
        public void Dispose()
        {
        }
    }
}