using System;

namespace Logging
{
    public class Logger
    {
        private static readonly Logger LoggerInstance = new Logger();
        private static LoggingLevel LogLevel;
        private Logger()
        {
            LogLevel = LoggingLevel.Normal;
        }

        public static Logger GetLogger()
        {
            return LoggerInstance;
        }
        
        public LoggingLevel LoggingLevel { get { return LogLevel; } set { LogLevel = value; } }

        public void Log(string message, LoggingLevel level = LoggingLevel.Normal)
        {
            Log(message, string.Empty, level);
        }

        public void Log(string message, string arg, LoggingLevel level = LoggingLevel.Normal)
        {
            Log(message, string.IsNullOrWhiteSpace(arg) ? new object[0] : new object[] {arg}, level);
        }

        public void Log(string message, string arg1, string arg2, LoggingLevel level = LoggingLevel.Normal)
        {
            Log(message, new object[] {arg1, arg2}, level);
        }

        public void Log(string message, string arg1, string arg2, string arg3, LoggingLevel level = LoggingLevel.Normal)
        {
            Log(message, new object[] { arg1, arg2, arg3 }, level);
        }

        public void Log(string message, object[] args, LoggingLevel level = LoggingLevel.Normal)
        {
            int logLevelVal = (int) LogLevel;
            int levelVal = (int) level;
            

            if (levelVal <= logLevelVal)
            {
                object[] data = args.Length == 0 ? new object[0] : new object[args.Length];
                
                for (int i = 0; i < args.Length; i++)
                {
                    data[i] = args[i] == null ? string.Empty : args[i].ToString();
                }

                if (data.Length == 0)
                {
                    data = new object[] { string.Empty };
                }

                Console.WriteLine(message, data);                
            }

        }
    }
}
