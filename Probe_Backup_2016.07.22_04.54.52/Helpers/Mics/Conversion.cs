using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Probe.Helpers.Mics
{
    public class ClientTimeZoneHelper
    {
        public static string ConvertToLocalTimeAndFormat(DateTime dt, string format)
        {
            var o = HttpContext.Current.Session["tzo"];
            var tzo = o == null ? 0 : Convert.ToDouble(o);

            dt = dt.AddMinutes(-1 * tzo);

            var s = dt.ToString(format);

            if (tzo == 0)
                s += " GMT";

            return s;
        }

        public static DateTime ConvertToLocalTime(DateTime dt, bool serverOveride)
        {


            double serverTimeOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalMinutes;

            //In the case where the server is on UTC datetime (Azure) - we don't need to readjust the date-times
            //that are read from the database. A UTC datetime from Azure will be serialized to a local datetime when converted to JSON format.
            if (serverTimeOffset != 0 || serverOveride)
            {
                var o = HttpContext.Current.Session["tzo"];
                var tzo = o == null ? 0 : Convert.ToDouble(o);

                dt = dt.AddMinutes(-1 * tzo);
            }

            return dt;
        }


        public static DateTime ConvertLocalToUTC(DateTime dt)
        {
            var o = HttpContext.Current.Session["tzo"];
            var tzo = o == null ? 0 : Convert.ToDouble(o);

            DateTime UTCDateTime = dt.AddMinutes(tzo);

            return UTCDateTime;
        }

    }
}