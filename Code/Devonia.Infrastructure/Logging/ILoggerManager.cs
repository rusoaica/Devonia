/// Written by: Yulia Danilova
/// Creation Date: 26th of January, 2021
/// Purpose: Interface for providing logging functionality
#region ========================================================================= USING =====================================================================================
using System;
#endregion

namespace Devonia.Infrastructure.Logging
{
    public interface ILoggerManager
    {
        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Writes the diagnostic message at Debug level
        /// </summary>
        /// <param name="message">The message to log</param>
        void LogDebug(string message);

        /// <summary>
        /// Writes the diagnostic exception at Debug level
        /// </summary>
        /// <param name="exception">The exception to log</param>
        void LogDebug(Exception exception);

        /// <summary>
        /// Writes the diagnostic message at Error level
        /// </summary>
        /// <param name="message">The exception to log</param>
        void LogError(string message);

        /// <summary>
        /// Writes the diagnostic exception at Error level
        /// </summary>
        /// <param name="exception">The exception to log</param>
        void LogError(Exception exception);

        /// <summary>
        /// Writes the diagnostic message at Info level
        /// </summary>
        /// <param name="message">The message to log</param>
        void LogInfo(string message);

        /// <summary>
        /// Writes the diagnostic exception at Info level
        /// </summary>
        /// <param name="exception">The exception to log</param>
        void LogInfo(Exception exception);

        /// <summary>
        /// Writes the diagnostic message at Warn level
        /// </summary>
        /// <param name="message">The message to log</param>
        void LogWarn(string message);

        /// <summary>
        /// Writes the diagnostic exception at Warn level
        /// </summary>
        /// <param name="exception">The exception to log</param>
        void LogWarn(Exception exception);
        #endregion
    }
}
