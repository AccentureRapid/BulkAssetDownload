using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulkAssetDownload
{
    public class Helpers
    {
        private static string LOG_AREA = "BULK ASSET DOWNLOAD";
        private static string LOG_CATEGORY = "BULK ASSET DOWNLOAD";

        public static void Log(Exception e)
        {
            try
            {
                Logger.LogToULS(0, SPTraceLogger.TraceSeverity.Exception, LOG_AREA, LOG_CATEGORY, e.ToString());
            }
            catch
            {
            }
        }

        public static void Log(string message)
        {
            try
            {

                Logger.LogToULS(0, SPTraceLogger.TraceSeverity.Verbose, LOG_AREA, LOG_CATEGORY, message);
            }
            catch
            {

            }
        }
    }
}
