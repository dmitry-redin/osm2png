using System;
using System.Collections.Generic;
using System.Linq;

namespace Osm2Png
{
    public static class WGS2UTMConverter
    {
        private static double a = 6378137;
        private static double eccSquared = 0.00669438;
        private static double toRadians(double grad)
        {
            return grad * Math.PI / 180;
        }
        public static void convertToUTM(double latitude, double longitude, ref double x, ref double y)
        {
            int ZoneNumber;

            var LongTemp = longitude;
            var LatRad = toRadians(latitude);
            var LongRad = toRadians(LongTemp);

            if (LongTemp >= 8 && LongTemp <= 13 && latitude > 54.5 && latitude < 58)
            {
                ZoneNumber = 32;
            }
            else if (latitude >= 56.0 && latitude < 64.0 && LongTemp >= 3.0 && LongTemp < 12.0)
            {
                ZoneNumber = 32;
            }
            else
            {
                ZoneNumber = (int) ((LongTemp + 180) / 6) + 1;

                if (latitude >= 72.0 && latitude < 84.0)
                {
                    if (LongTemp >= 0.0 && LongTemp < 9.0)
                    {
                        ZoneNumber = 31;
                    }
                    else if (LongTemp >= 9.0 && LongTemp < 21.0)
                    {
                        ZoneNumber = 33;
                    }
                    else if (LongTemp >= 21.0 && LongTemp < 33.0)
                    {
                        ZoneNumber = 35;
                    }
                    else if (LongTemp >= 33.0 && LongTemp < 42.0)
                    {
                        ZoneNumber = 37;
                    }
                }
            }

            var LongOrigin = (ZoneNumber - 1) * 6 - 180 + 3;
            var LongOriginRad = toRadians(LongOrigin);

            var eccPrimeSquared = (eccSquared) / (1 - eccSquared);

            var N = a / Math.Sqrt(1 - eccSquared * Math.Sin(LatRad) * Math.Sin(LatRad));
            var T = Math.Tan(LatRad) * Math.Tan(LatRad);
            var C = eccPrimeSquared * Math.Cos(LatRad) * Math.Cos(LatRad);
            var A = Math.Cos(LatRad) * (LongRad - LongOriginRad);

            var M = a * ((1 - eccSquared / 4 - 3 * eccSquared * eccSquared / 64 - 5 * eccSquared * eccSquared * eccSquared / 256) * LatRad
                    - (3 * eccSquared / 8 + 3 * eccSquared * eccSquared / 32 + 45 * eccSquared * eccSquared * eccSquared / 1024) * Math.Sin(2 * LatRad)
                    + (15 * eccSquared * eccSquared / 256 + 45 * eccSquared * eccSquared * eccSquared / 1024) * Math.Sin(4 * LatRad)
                    - (35 * eccSquared * eccSquared * eccSquared / 3072) * Math.Sin(6 * LatRad));

            var UTMEasting = 0.9996 * N * (A + (1 - T + C) * A * A * A / 6
                    + (5 - 18 * T + T * T + 72 * C - 58 * eccPrimeSquared) * A * A * A * A * A / 120)
                    + 500000.0;

            var UTMNorthing = 0.9996 * (M + N * Math.Tan(LatRad) * (A * A / 2 + (5 - T + 9 * C + 4 * C * C) * A * A * A * A / 24
                    + (61 - 58 * T + T * T + 600 * C - 330 * eccPrimeSquared) * A * A * A * A * A * A / 720));

            if (latitude < 0)
                UTMNorthing += 10000000.0;

            (x,y) = (UTMEasting, UTMNorthing);
        }
    }
}