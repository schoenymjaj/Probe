using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Probe.Helpers.Mics
{
    /*
     * Read the text from a SQL script placed in the ~/Scripts/sqlscript folder
     */
    public class ReadSQLScriptFile
    {
        static public string Read(string filePath)
        {
            String path;
            if (HttpContext.Current.Server != null)
            {
                path = HttpContext.Current.Server.MapPath("/Scripts/sqlscript/" + filePath);
            }
            else
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), "/Scripts/sqlscript/" + filePath);
            }

            var file = File.OpenText(path);

            string sqlText = file.ReadToEnd();

            return sqlText;

        }
    }
}