using System;
using System.Reflection;
using log4net;

namespace Infrastructure.Data.Utilities
{

    public static class DataParser
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 
        /// Get a booean from a string
        /// </summary>
        /// <param name="value">Value to evaluate</param>
        /// <returns>A boolean</returns>
        public static bool GetBoolean(string value)
        {
            value = value.Trim();

            return value.Equals("Y", StringComparison.OrdinalIgnoreCase)
                || value.Equals("1", StringComparison.OrdinalIgnoreCase)
                || value.Equals("TRUE", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Get an int from string
        /// </summary>
        /// <param name="value">Value to be parsed</param>
        /// <returns>If can parse returns value else returns 0</returns>
        public static int GetInt(string value)
        {

            int r;

            bool isConverted = Int32.TryParse(value, out r);

            if (!isConverted)
            {
                _log.Debug("Error while trying convert " + value + " to an integer.");
            }

            return r;

        }

        /// <summary>
        /// Get a long from string
        /// </summary>
        /// <param name="value">Value to be parsed</param>
        /// <returns>If can parse returns value else returns 0</returns>
        public static long GetLong(string value)
        {

            long r;

            bool isConverted = Int64.TryParse(value, out r);

            if (!isConverted)
            {
                _log.Debug("Error while trying convert " + value + " to a long.");
            }

            return r;

        }

        /// <summary>
        /// Get Double from a string
        /// </summary>
        /// <param name="value">Value to be parsed</param>
        /// <returns>If parsed returns the value else returns 0.0</returns>
        public static double GetDouble(string value)
        {

            double r;

            bool isConverted = Double.TryParse(value, out r);

            if (!isConverted)
            {
                _log.Debug("Error while trying convert " + value + " to a double.");
            }

            return r;

        }

        /// <summary>
        /// Get the DateTime from a string. Will return current DateTime or NULL if parse fails.
        /// </summary>
        /// <param name="value">Value to be parsed</param>
        /// <param name="useNow">If true, return current date time</param>
        /// <returns>If parsed returns DateTime value else returns DateTime.Now or NULL</returns>
        public static DateTime? GetDateTime(string value, bool useNow = false)
        {
            DateTime d;
            var isConverted = DateTime.TryParse(value, out d);

            if (!isConverted)
            {
                _log.Debug("Error while trying convert " + value + " to a DateTime.");
                if (useNow)
                {
                    return DateTime.Now;
                }

                return null;
            }

            return d;
        }      
    }
}