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

        public static DateTime ConvertToLocalTime(DateTime dt)
        {


            double serverTimeOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalMinutes;

            //In the case where the server is on UTC time (Azure) - we don't need to readjust the date-times
            //that are read from the database.
            if (serverTimeOffset != 0)
            {
                var o = HttpContext.Current.Session["tzo"];
                var tzo = o == null ? 0 : Convert.ToDouble(o);

                dt = dt.AddMinutes(-1 * tzo);
            }

            return dt;
        }


        //public static DateTime ConvertToLocalTime(DateTime dt)
        //{

        //    double serverTimeOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalMinutes;

        //    var o = HttpContext.Current.Session["tzo"];
        //    var clientTimeOffset = o == null ? 0 : Convert.ToDouble(o); //Client Offset from UTC

        //    double totalTimeOffset = serverTimeOffset - (-1 * clientTimeOffset);

        //    dt = dt.AddMinutes(-1 * totalTimeOffset);

        //    return dt;
        //}

        public static DateTime ConvertLocalToUTC(DateTime dt)
        {
            var o = HttpContext.Current.Session["tzo"];
            var tzo = o == null ? 0 : Convert.ToDouble(o);

            DateTime UTCDateTime = dt.AddMinutes(tzo);

            return UTCDateTime;
        }

    }
}