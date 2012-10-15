using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Microsoft.SharePoint;

namespace BulkAssetDownload
{
    /// <summary>
    /// Logger class for events and exception
    /// </summary>
    public static class Logger
    {

        #region [Constructor]
        /// <summary>
        /// Constructor
        /// </summary>
        static Logger()
        {
            
        }
        #endregion [Constructor]

        #region [LogException]

        /// <summary>
        /// Logs exceptions
        /// </summary>
        /// <param name="exception">Exception to be logged</param>
        public static void LogException(Exception exception)
        {
            LogExceptionToULS(exception.ToString());
        }

        /// <summary>
        /// Logs exceptions
        /// </summary>
        /// <param name="exceptionMessage">Exception message to be logged</param>
        public static void LogException(string exceptionMessage)
        {
            try
            {
                LogExceptionToULS(exceptionMessage);
            }
            catch(Exception e)
            {
                LogLoggerException(e.ToString());
            };
        }

        private static void LogExceptionToULS(string exceptionMessage)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPTraceLogger traceLogger = new SPTraceLogger())
                    {
                        traceLogger.Write(0, SPTraceLogger.TraceSeverity.Exception,
                            "Logger", "Exception", exceptionMessage);
                    }
                });
            }
            catch(Exception e)
            {
                LogLoggerException(e.ToString());
            };
        }

        #endregion [LogException]

        #region [LogEvent]

        /// <summary>
        /// Logs events
        /// </summary>
        /// <param name="message">Message to be logged</param>
        public static void LogEvent(string message)
        {
            LogEventToULS(message);
        }

        private static void LogEventToULS(string message)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPTraceLogger traceLogger = new SPTraceLogger())
                    {
                        traceLogger.Write(0, SPTraceLogger.TraceSeverity.InformationEvent,
                            "Logger", "Application", message);
                    }
                });
            }
            catch(Exception e)
            {
                LogLoggerException(e.ToString());
            };
        }

        #endregion [LogEvent]

        #region LogToULS


        /// <summary>
        /// Method to log only to the ULS
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="severity"></param>
        /// <param name="area"></param>
        /// <param name="category"></param>
        /// <param name="message"></param>
        public static void LogToULS(uint eventId, SPTraceLogger.TraceSeverity severity, string area, string category, string message)
        {
            try
            {

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPTraceLogger traceLogger = new SPTraceLogger())
                    {
                        traceLogger.Write(eventId, severity, area, category, message);
                    }
                });
            }
            catch(Exception e)
            {
                LogLoggerException(e.ToString());
            }
        }
        #endregion

        #region [LogLoggerException]

        /// <summary>
        /// Logs exception
        /// </summary>
        /// <param name="message">message to log</param>
        internal static void LogLoggerException(string message)
        {
            try
            {
                if (!System.Diagnostics.EventLog.SourceExists("Logger"))
                {
                    System.Diagnostics.EventLog.CreateEventSource("Logger", "Application");
                }

                using (EventLog eventLog = new EventLog())
                {
                    eventLog.Source = "Logger";
                    eventLog.WriteEntry(message);
                }
            }
            catch
            {
            }
        }

        #endregion [LogLoggerException]
    }
}
