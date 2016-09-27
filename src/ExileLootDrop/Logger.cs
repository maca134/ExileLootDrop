using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ExileLootDrop
{
    internal static class Logger
    {
        private static readonly LogPublisher LogPublisher;

        private static readonly object Sync = new object();
        private static bool _isTurned = true;
        private static bool _isTurnedDebug = true;

        static Logger()
        {
            lock (Sync)
            {
                LogPublisher = new LogPublisher();
                Modules = new ModuleManager();
                Debug = new DebugLogger();
            }
        }

        internal static Level DefaultLevel { get; set; } = Level.Info;

        internal static ILoggerHandlerManager LoggerHandlerManager => LogPublisher;

        internal static IEnumerable<LogMessage> Messages => LogPublisher.Messages;

        internal static DebugLogger Debug { get; }

        internal static ModuleManager Modules { get; }

        internal static void Log()
        {
            Log("There is no message");
        }

        internal static void Log(string message)
        {
            Log(DefaultLevel, message);
        }

        internal static void Log(Level level, string message)
        {
            var stackFrame = FindStackFrame();
            var methodBase = GetCallingMethodBase(stackFrame);
            var callingMethod = methodBase.Name;
            var callingClass = methodBase.ReflectedType?.Name;
            var lineNumber = stackFrame.GetFileLineNumber();

            Log(level, message, callingClass, callingMethod, lineNumber);
        }

        internal static void Log(Exception exception)
        {
            Log(Level.Error, exception.Message);
            Modules.ExceptionLog(exception);
        }

        internal static void Log<TClass>(Exception exception) where TClass : class
        {
            var message = $"Log exception -> Message: {exception.Message}\nStackTrace: {exception.StackTrace}";
            Log<TClass>(Level.Error, message);
        }

        internal static void Log<TClass>(string message) where TClass : class
        {
            Log<TClass>(DefaultLevel, message);
        }

        internal static void Log<TClass>(Level level, string message) where TClass : class
        {
            var stackFrame = FindStackFrame();
            var methodBase = GetCallingMethodBase(stackFrame);
            var callingMethod = methodBase.Name;
            var callingClass = typeof (TClass).Name;
            var lineNumber = stackFrame.GetFileLineNumber();

            Log(level, message, callingClass, callingMethod, lineNumber);
        }

        private static void Log(Level level, string message, string callingClass, string callingMethod, int lineNumber)
        {
            if (!_isTurned || (!_isTurnedDebug && level == Level.Debug))
                return;

            var currentDateTime = DateTime.Now;

            Modules.BeforeLog();
            var logMessage = new LogMessage(level, message, currentDateTime, callingClass, callingMethod, lineNumber);
            LogPublisher.Publish(logMessage);
            Modules.AfterLog(logMessage);
        }

        private static MethodBase GetCallingMethodBase(StackFrame stackFrame)
        {
            return stackFrame == null
                ? MethodBase.GetCurrentMethod()
                : stackFrame.GetMethod();
        }

        private static StackFrame FindStackFrame()
        {
            var stackTrace = new StackTrace();
            var frames = stackTrace.GetFrames();
            if (frames == null)
                return null;
            for (var i = 0; i < frames.Length; i++)
            {
                var methodBase = stackTrace.GetFrame(i).GetMethod();
                var name = MethodBase.GetCurrentMethod().Name;
                if (!methodBase.Name.Equals("Log") && !methodBase.Name.Equals(name))
                    return new StackFrame(i, true);
            }
            return null;
        }

        internal static void On()
        {
            _isTurned = true;
        }

        internal static void Off()
        {
            _isTurned = false;
        }

        internal static void DebugOn()
        {
            _isTurnedDebug = true;
        }

        internal static void DebugOff()
        {
            _isTurnedDebug = false;
        }

        internal enum Level
        {
            None,
            Info,
            Warning,
            Error,
            Severe,
            Fine,
            Debug
        }
    }

    internal interface ILoggerFormatter
    {
        string ApplyFormat(LogMessage logMessage);
    }

    internal interface ILoggerHandlerManager
    {
        ILoggerHandlerManager AddHandler(ILoggerHandler loggerHandler);

        bool RemoveHandler(ILoggerHandler loggerHandler);
    }

    internal interface ILoggerHandler
    {
        void Publish(LogMessage logMessage);
    }

    internal class LogMessage
    {
        public LogMessage()
        {
        }

        public LogMessage(Logger.Level level, string text, DateTime dateTime, string callingClass, string callingMethod,
            int lineNumber)
        {
            Level = level;
            Text = text;
            DateTime = dateTime;
            CallingClass = callingClass;
            CallingMethod = callingMethod;
            LineNumber = lineNumber;
        }

        public DateTime DateTime { get; set; }
        public Logger.Level Level { get; set; }
        public string Text { get; set; }
        public string CallingClass { get; set; }
        public string CallingMethod { get; set; }
        public int LineNumber { get; set; }

        public override string ToString()
        {
            return new DefaultLoggerFormatter().ApplyFormat(this);
        }
    }

    internal class DefaultLoggerFormatter : ILoggerFormatter
    {
        public string ApplyFormat(LogMessage logMessage)
        {
            return
                $"{logMessage.DateTime:dd.MM.yyyy HH:mm}: {logMessage.Level} [line: {logMessage.LineNumber} {logMessage.CallingClass} -> {logMessage.CallingMethod}()]: {logMessage.Text}";
        }
    }

    internal class DebugLogger
    {
        private const Logger.Level DebugLevel = Logger.Level.Debug;

        public void Log()
        {
            Log("There is no message");
        }

        public void Log(string message)
        {
            Logger.Log(DebugLevel, message);
        }

        public void Log(Exception exception)
        {
            Logger.Log(DebugLevel, exception.Message);
        }

        public void Log<TClass>(Exception exception) where TClass : class
        {
            var message = $"Log exception -> Message: {exception.Message}\nStackTrace: {exception.StackTrace}";
            Logger.Log<TClass>(DebugLevel, message);
        }

        public void Log<TClass>(string message) where TClass : class
        {
            Logger.Log<TClass>(DebugLevel, message);
        }
    }

    internal class ModuleManager
    {
        private readonly IDictionary<string, LoggerModule> _modules;

        public ModuleManager()
        {
            _modules = new Dictionary<string, LoggerModule>();
        }

        public void BeforeLog()
        {
            foreach (var loggerModule in _modules.Values)
                loggerModule.BeforeLog();
        }

        public void AfterLog(LogMessage logMessage)
        {
            foreach (var loggerModule in _modules.Values)
                loggerModule.AfterLog(logMessage);
        }

        public void ExceptionLog(Exception exception)
        {
            foreach (var loggerModule in _modules.Values)
                loggerModule.ExceptionLog(exception);
        }

        public void Install(LoggerModule module)
        {
            while (true)
            {
                if (!_modules.ContainsKey(module.Name))
                {
                    module.Initialize();
                    _modules.Add(module.Name, module);
                }
                else
                {
                    // reinstall module
                    Uninstall(module.Name);
                    continue;
                }
                break;
            }
        }

        public void Uninstall(LoggerModule module)
        {
            if (_modules.ContainsKey(module.Name))
                _modules.Remove(module.Name);
        }

        public void Uninstall(string moduleName)
        {
            if (_modules.ContainsKey(moduleName))
                _modules.Remove(moduleName);
        }
    }

    internal abstract class LoggerModule
    {
        public abstract string Name { get; }

        public virtual void BeforeLog()
        {
        }

        public virtual void AfterLog(LogMessage logMessage)
        {
        }

        public virtual void ExceptionLog(Exception exception)
        {
        }

        public virtual void Initialize()
        {
        }
    }

    internal class LogPublisher : ILoggerHandlerManager
    {
        private readonly IList<ILoggerHandler> _loggerHandlers;
        private readonly IList<LogMessage> _messages;

        public LogPublisher()
        {
            _loggerHandlers = new List<ILoggerHandler>();
            _messages = new List<LogMessage>();
        }

        public IEnumerable<LogMessage> Messages => _messages;

        public ILoggerHandlerManager AddHandler(ILoggerHandler loggerHandler)
        {
            if (loggerHandler != null)
                _loggerHandlers.Add(loggerHandler);
            return this;
        }

        public bool RemoveHandler(ILoggerHandler loggerHandler)
        {
            return _loggerHandlers.Remove(loggerHandler);
        }

        public void Publish(LogMessage logMessage)
        {
            _messages.Add(logMessage);
            foreach (var loggerHandler in _loggerHandlers)
                loggerHandler.Publish(logMessage);
        }
    }

    internal class ConsoleLoggerHandler : ILoggerHandler
    {
        private readonly ILoggerFormatter _loggerFormatter;

        internal ConsoleLoggerHandler() : this(new DefaultLoggerFormatter())
        {
        }

        internal ConsoleLoggerHandler(ILoggerFormatter loggerFormatter)
        {
            _loggerFormatter = loggerFormatter;
        }

        public void Publish(LogMessage logMessage)
        {
            Console.WriteLine(_loggerFormatter.ApplyFormat(logMessage));
        }
    }

    internal class FileLoggerHandler : ILoggerHandler
    {
        private static readonly object Sync = new object();
        private readonly string _directory;
        private readonly string _fileName;
        private readonly ILoggerFormatter _loggerFormatter;

        internal FileLoggerHandler() : this(CreateFileName())
        {
        }

        internal FileLoggerHandler(string fileName) : this(fileName, string.Empty)
        {
        }

        internal FileLoggerHandler(string fileName, string directory)
            : this(new DefaultLoggerFormatter(), fileName, directory)
        {
        }

        internal FileLoggerHandler(ILoggerFormatter loggerFormatter) : this(loggerFormatter, CreateFileName())
        {
        }

        internal FileLoggerHandler(ILoggerFormatter loggerFormatter, string fileName)
            : this(loggerFormatter, fileName, string.Empty)
        {
        }

        internal FileLoggerHandler(ILoggerFormatter loggerFormatter, string fileName, string directory)
        {
            _loggerFormatter = loggerFormatter;
            _fileName = fileName;
            _directory = directory;
        }

        public void Publish(LogMessage logMessage)
        {
            if (!string.IsNullOrEmpty(_directory))
            {
                var directoryInfo = new DirectoryInfo(Path.Combine(_directory));
                if (!directoryInfo.Exists)
                    directoryInfo.Create();
            }

            try
            {
                lock (Sync)
                {
                    using (
                        var writer = new StreamWriter(File.Open(Path.Combine(_directory, _fileName), FileMode.Append)))
                        writer.WriteLine(_loggerFormatter.ApplyFormat(logMessage));
                }
            }
            catch
            {
                // ignore
            }
        }

        private static string CreateFileName()
        {
            var currentDate = DateTime.Now;
            var guid = Guid.NewGuid();
            return
                $"Log_{currentDate.Year:0000}{currentDate.Month:00}{currentDate.Day:00}-{currentDate.Hour:00}{currentDate.Minute:00}_{guid}.log";
        }
    }
}