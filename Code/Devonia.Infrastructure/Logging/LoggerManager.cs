/// Written by: Yulia Danilova
/// Creation Date: 26th of January, 2021
/// Purpose: Implementation of ILoggerManager interface
#region ========================================================================= USING =====================================================================================
using NLog;
using System;
using NLog.Config;
using NLog.Targets;
#endregion

namespace Devonia.Infrastructure.Logging
{
    public class LoggerManager : ILoggerManager
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Default C-tor
        /// </summary>
        public LoggerManager()
        {
            SetLoggingEnvironment();
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Initializes NLog
        /// </summary>
        private void SetLoggingEnvironment()
        {
            // create NLog configuration  
            LoggingConfiguration _config = new LoggingConfiguration();
            FileTarget _file_target = new FileTarget("target2")
            {
                FileName = (AppDomain.CurrentDomain.BaseDirectory + @"Logs\" + DateTime.Now.ToString("yyMMdd") + ".log").Replace("\\", "/"),
                Layout = "${longdate} ${level} ${message}  ${exception}"
            };
            _config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, _file_target));
            _config.AddTarget(_file_target);
            LogManager.Configuration = _config;
            // enable when debugging NLog itself:
            // LogManager.ThrowExceptions = true;
        }

        /// <summary>
        /// Writes the diagnostic message at Debug level
        /// </summary>
        /// <param name="message">The message to log</param>
        public void LogDebug(string message)
        {
            logger.Debug(message + Environment.NewLine);
        }

        /// <summary>
        /// Writes the diagnostic exception at Debug level
        /// </summary>
        /// <param name="exception">The exception to log</param>
        public void LogDebug(Exception exception)
        {
            logger.Debug(exception + Environment.NewLine);
        }

        /// <summary>
        /// Writes the diagnostic message at Error level
        /// </summary>
        /// <param name="message">The exception to log</param>
        public void LogError(string message)
        {
            logger.Error(message + Environment.NewLine);
        }

        /// <summary>
        /// Writes the diagnostic exception at Error level
        /// </summary>
        /// <param name="exception">The exception to log</param>
        public void LogError(Exception exception)
        {
            logger.Error(exception + Environment.NewLine);
        }

        /// <summary>
        /// Writes the diagnostic message at Info level
        /// </summary>
        /// <param name="message">The message to log</param>
        public void LogInfo(string message)
        {
            logger.Info(message + Environment.NewLine);
        }

        /// <summary>
        /// Writes the diagnostic exception at Info level
        /// </summary>
        /// <param name="exception">The exception to log</param>
        public void LogInfo(Exception exception)
        {
            logger.Info(exception + Environment.NewLine);
        }

        /// <summary>
        /// Writes the diagnostic message at Warn level
        /// </summary>
        /// <param name="message">The message to log</param>
        public void LogWarn(string message)
        {
            logger.Warn(message + Environment.NewLine);
        }

        /// <summary>
        /// Writes the diagnostic exception at Warn level
        /// </summary>
        /// <param name="exception">The exception to log</param>
        public void LogWarn(Exception exception)
        {
            logger.Warn(exception + Environment.NewLine);
        }
        #endregion
    }
}
