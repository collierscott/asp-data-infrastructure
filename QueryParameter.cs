using System.Data;

namespace Infrastructure.Data
{
    /// <summary>
    /// A parameter for a query
    /// </summary>
    public class QueryParameter
    {
        public string ParameterName { get; set; }
        public string Value { get; set; }
        public DbType DbType { get; set; }

        public QueryParameter()
        {
            DbType = DbType.String;
        }

    }

}
