using System.Reflection;

namespace Infrastructure.Data
{

    public class DataReaderColumn
    {

        public string DbColName;
        public int DbColOrdinal;
        public PropertyInfo PropertyInfo;

        /// <summary>
        /// A representation on a column in a DataReader
        /// </summary>
        /// <param name="dbColName">Name of the column</param>
        /// <param name="dbColOrdinal">Ordinal of the column</param>
        /// <param name="propertyInfo">Property of te column</param>
        public DataReaderColumn(string dbColName, int dbColOrdinal, PropertyInfo propertyInfo)
        {
            DbColName = dbColName;
            DbColOrdinal = dbColOrdinal;
            PropertyInfo = propertyInfo;
        }

    }

}
