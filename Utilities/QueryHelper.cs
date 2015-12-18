using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.Utilities
{
    public static class QueryHelper
    {

        /// <summary>
        /// Add single quotes to elements of a comma delimited string
        /// </summary>
        /// <param name="value">Comma delimited string to have single quotes added to it</param>
        /// <returns>String with single quotes added</returns>
        public static string AddSingleQuotes(string value)
        {

            if (string.IsNullOrEmpty(value)) return "";

            value = value.Trim();

            var sb = new StringBuilder();

            string[] split = value.Split(',');

            for (var i = 0; i < split.Length; i++)
            {
                if (split[i].Length > 0)
                {
                    sb.Append("'" + split[i] + "'");

                    if (i < split.Length - 1)
                    {
                        sb.Append(",");
                    }

                }
            }

            return sb.ToString();

        }

        /// <summary>
        /// Add single quotes to elements in a list
        /// </summary>
        /// <param name="list">A list of strings that need single quotes added</param>
        /// <returns>A string of comma delimited values with single quotes added</returns>
        public static string AddSingleQuotes(List<string> list)
        {

            if (list == null || list.Count == 0) return "";

            var sb = new StringBuilder();

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Length > 0)
                {
                    sb.Append("'" + list[i] + "'");

                    if (i < list.Count - 1)
                    {
                        sb.Append(",");
                    }

                }
            }

            return sb.ToString();

        }

         
    }
}