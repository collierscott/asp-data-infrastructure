using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data.Utilities
{

#pragma warning disable 1570
    /// <summary>
    /// Compares a alphanumeric string in order to sort it properly
    /// 
    /// Usage: Using the IEnumerable OrderBy method. OrderBy(o => o.ThingToSortBy, new AlphanumComparator())
    /// Example:  
    ///     var tools = (from row in data.AsEnumerable()
    ///         where facility.Equals(row.Field<string>("FACILITY")) && mod2.Id.Equals(row.Field<string>("MODULE")) && ts1.Id.Equals(row.Field<string>("TOOLSET"))
    ///         select new Tool
    ///         {
    ///             Id = row.Field<string>("TOOL"),
    ///             Name = row.Field<string>("TOOL_DISPLAY"),
    ///             FacilityId = row.Field<string>("FACILITY"),
    ///             ModuleId = row.Field<string>("MODULE"),
    ///             EquipmentFamilyId = row.Field<string>("TOOLSET")
    ///         }).Distinct(new PropertyComparer<Tool>("Id")).OrderBy(o => o.Name, new AlphanumComparator()).ToList();
    /// </summary>
#pragma warning restore 1570
    public class AlphaNumComparator : IComparer<string>
    {

        private enum ChunkType { Alphanumeric, Numeric };

        private bool InChunk(char ch, char otherCh)
        {
            var type = ChunkType.Alphanumeric;

            if (char.IsDigit(otherCh))
            {
                type = ChunkType.Numeric;
            }

            if ((type == ChunkType.Alphanumeric && char.IsDigit(ch))
                || (type == ChunkType.Numeric && !char.IsDigit(ch)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Compares two strings
        /// </summary>
        /// <param name="x">String to compare</param>
        /// <param name="y">String to compare</param>
        /// <returns>0 if strings are equal, 1 if x > y, -1 if y > x</returns>
        public int Compare(string x, string y)
        {
            var s1 = x;
            var s2 = y;

            if (string.IsNullOrWhiteSpace(s1) || string.IsNullOrWhiteSpace(s2))
            {
                return 0;
            }

            var thisMarker = 0;
            var thatMarker = 0;

            while ((thisMarker < s1.Length) || (thatMarker < s2.Length))
            {
                if (thisMarker >= s1.Length)
                {
                    return -1;
                }
                
                if (thatMarker >= s2.Length)
                {
                    return 1;
                }

                var thisCh = s1[thisMarker];
                var thatCh = s2[thatMarker];

                var thisChunk = new StringBuilder();
                var thatChunk = new StringBuilder();

                while ((thisMarker < s1.Length) && (thisChunk.Length == 0 || InChunk(thisCh, thisChunk[0])))
                {
                    thisChunk.Append(thisCh);
                    thisMarker++;

                    if (thisMarker < s1.Length)
                    {
                        thisCh = s1[thisMarker];
                    }
                }

                while ((thatMarker < s2.Length) && (thatChunk.Length == 0 || InChunk(thatCh, thatChunk[0])))
                {
                    thatChunk.Append(thatCh);
                    thatMarker++;

                    if (thatMarker < s2.Length)
                    {
                        thatCh = s2[thatMarker];
                    }
                }

                var result = 0;

                // If both chunks contain numeric characters, sort them numerically
                if (char.IsDigit(thisChunk[0]) && char.IsDigit(thatChunk[0]))
                {
                    var thisNumericChunk = Convert.ToInt32(thisChunk.ToString());
                    var thatNumericChunk = Convert.ToInt32(thatChunk.ToString());

                    if (thisNumericChunk < thatNumericChunk)
                    {
                        result = -1;
                    }

                    if (thisNumericChunk > thatNumericChunk)
                    {
                        result = 1;
                    }
                }
                else
                {
                    result = String.Compare(thisChunk.ToString(), thatChunk.ToString(), StringComparison.Ordinal);
                }

                if (result != 0)
                {
                    return result;
                }
            }

            return 0;
        }

    }
}
