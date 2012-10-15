using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulkAssetDownload
{
    public class ConfigurationManager
    {
        internal static string GetSetting(string p)
        {
            return "100";
        }

        internal static string GetRichTextSetting(string p)
        {
            return "By downloading to this file, you agree to our terms and conditions";
        }
    }
}
