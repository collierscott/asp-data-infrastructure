using System;
using System.Collections.Generic;
using System.Data;
using Infrastructure.Data.Abstract;
using Infrastructure.Data.Notify;
using log4net;

namespace Infrastructure.Data
{

    public abstract class SqlQuery : ISqlQuery
    {

        // ReSharper disable once InconsistentNaming
        private readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Notification _message;
        public string Id { get; set; }
        public string Query { get; set; }
        public IList<QueryParameter> Parameters { get; set; }
        public ObjectMapper ObjectMap { get; set; }

        /// <summary>
        /// Used to return a user readable sql string
        /// </summary>
        public Notification Message
        {
            get
            {
                _message.Id = Id;
                _message.UserMessage = ToString();
                return _message;
            }
            set
            {
                _message = value;
            }
        }

        /// <summary>
        /// A sql query
        /// </summary>
        protected SqlQuery()
        {

            Parameters = new List<QueryParameter>();
            ObjectMap = new ObjectMapper();
            _message = new Notification
            {
                Id = Id,
                Type = NotificationType.Query
            };
        }

        /// <summary>
        /// Takes a SQL query and optional parameters and converts it into a user readable query string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {

            var commandTxt = Query;

            if (Parameters != null)
            {

                foreach (var parms in Parameters)
                {

                    var val = String.Empty;

                    if (parms.DbType.Equals(DbType.String) || parms.DbType.Equals(DbType.DateTime))
                    {
                        val = "'" + Convert.ToString(parms.Value).Replace(@"\", @"\\").Replace("'", @"\'") + "'";
                    }

                    if (parms.DbType.Equals(DbType.Int16) || parms.DbType.Equals(DbType.Int32) || parms.DbType.Equals(DbType.Int64) || parms.DbType.Equals(DbType.Decimal) || parms.DbType.Equals(DbType.Double))
                    {
                        val = Convert.ToString(parms.Value);
                    }

                    var paramname = ":" + parms.ParameterName;

                    commandTxt = commandTxt.Replace(paramname, val);
                }

            }

            return commandTxt;

        }

    }

}
