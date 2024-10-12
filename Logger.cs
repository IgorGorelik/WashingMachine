using Serilog;
using System;
using System.IO;
using System.Windows.Forms;

namespace WashingMachine
{
    public sealed class Logger
    {
        #region Properties
        private static readonly Lazy<Logger> singletonInstance = new Lazy<Logger>(() => new Logger());
        private static volatile object locker = new object();
        public static Logger Instance { get { lock (locker) { return singletonInstance.Value; } } }
        #endregion

        #region Construction
        private Logger() { }
        public void Initialize()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(Application.StartupPath, "Logs", "WashingMachine.log"))
                .WriteTo.Console()
                .CreateLogger();

            LogSeparator(); // Empty line to separate applicatiopn runs.
        }
        #endregion

        #region Methods
        public void LogSeparator()
        {
            Log.Information("");
        }
        public void LogInformation(string logMessage)
        {
            Log.Information(logMessage);
        }

        public void LogWarning(string logMessage)
        {
            Log.Warning(logMessage);
        }

        public void LogError(Exception ex)
        {
            Log.Error(ex.Message); 
        }

        public void LogError(string logMessage)
        {
            Log.Error(logMessage);
        }
        #endregion
    }

}
