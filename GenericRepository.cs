using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Infrastructure.Data.Abstract;
using Infrastructure.Data.Notify;
using log4net;
using System;
using System.Text;

namespace Infrastructure.Data
{

    /// 'Get' methods get one or more basic entities from the database. 
    /// 'Build' methods build an object and may include other objects that it owns. Example:  BuildTools would create a tool and get all of its chambers, ports, and lots if needed
    /// 'Find' methods gets only one entity form the database
    public abstract class GenericRepository: IRepository
    {

        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once InconsistentNaming
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly string _connectionStringName;
        private readonly IDatabaseContext _context;
        private readonly IDbConnection _connection;

        // ReSharper disable once NotAccessedField.Local
        private bool _disposed;
        private IUnitOfWork _unitOfWork;

        public Notifications Messages { get; set; }


        //TODO Look into this. Not tested. Scott Collier 10/27/2014
        public IUnitOfWork UnitOfWork
        {
            get { return _unitOfWork ?? (_unitOfWork = new UnitOfWork(_context)); }
        }

        protected GenericRepository() : this(string.Empty) { }

        /// <summary>
        /// Generic Repository construtor
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string to use</param>
        protected GenericRepository(string connectionStringName)
        {

            Messages = new Notifications();
            
            _connectionStringName = connectionStringName;

            if (_context == null)
            {
                if (!string.IsNullOrWhiteSpace(_connectionStringName))
                {
                    _context = new DatabaseContext(_connectionStringName);
                    _connection = _context.Connection;
                    Messages = _context.Messages;

                }
            }
            else
            {
                
                _connection = _context.Connection;
            }
            
        }

        /// <summary>
        /// Generic Repository construtor
        /// </summary>
        /// <param name="context">DatabaseContext to use</param>
        protected GenericRepository(IDatabaseContext context)
        {

            Messages = new Notifications();
            _context = context;
            _connection = context.Connection;

        }

        /// <summary>
        /// Insert an item into the database
        /// </summary>
        /// <typeparam name="TEntity">Object Type</typeparam>
        /// <param name="entity">Item to insert into the database</param>
        /// <param name="query">The query used to insert item into the database</param>
        /// <param name="timeout"></param>
        /// <returns>How many items were inserted</returns>
        public int Insert<TEntity>(TEntity entity, SqlQuery query, int timeout = 30) where TEntity : class
        {

            int result = -1;

            try
            {
                using (var cmd = _connection.CreateCommand())
                {

                    BuildCommand(query, cmd, timeout);
                    result = cmd.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {

                _log.Error("An error occurred Insert<TEntity>. " + query + " " + ex);

                Messages.Add(new ErrorNotification
                {
                    Id = "Insert<TEntity>",
                    ExceptionText = ex.ToString(),
                    MessageException = ex,
                    Source = ex.Source,
                    StackTrace = ex.StackTrace,
                    Type = NotificationType.Error,
                    UserMessage = "An error occurred while retrieving data. " + query.Query

                });
            }

            return result;

        }

        /// <summary>
        /// Update an item in the database
        /// </summary>
        /// <typeparam name="TEntity">Object Type</typeparam>
        /// <param name="entity">Item to update</param>
        /// <param name="query">The query used to update an item</param>
        /// <param name="timeout"></param>
        /// <returns>How many items were updated</returns>
        public int Update<TEntity>(TEntity entity, SqlQuery query, int timeout = 30) where TEntity : class
        {
            int result = -1;

            try
            {
                using (var cmd = _connection.CreateCommand())
                {

                    BuildCommand(query, cmd, timeout);
                    result = cmd.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {

                _log.Error("An error occurred Insert<TEntity>. " + query + " " + ex);

                Messages.Add(new ErrorNotification
                {
                    Id = "Update<TEntity>",
                    ExceptionText = ex.ToString(),
                    MessageException = ex,
                    Source = ex.Source,
                    StackTrace = ex.StackTrace,
                    Type = NotificationType.Error,
                    UserMessage = "An error occurred while retrieving data. " + query.Query

                });
            }

            return result;
        }

        /// <summary>
        /// Delete an item from the database
        /// </summary>
        /// <typeparam name="TEntity">Object Type</typeparam>
        /// <param name="id">Id of object to be deleted</param>
        /// <param name="query">The query used to delete the item</param>
        /// <param name="timeout"></param>
        /// <returns>How many items were affected</returns>
        public int Delete<TEntity>(string id, SqlQuery query, int timeout = 30) where TEntity : class
        {
            int result = -1;

            try
            {
                using (var cmd = _connection.CreateCommand())
                {
                    
                    BuildCommand(query, cmd, timeout);
                    result = cmd.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {

                _log.Error("An error occurred Insert<TEntity>. " + query + " " + ex);

                Messages.Add(new ErrorNotification
                {
                    Id = "Delete<TEntity>",
                    ExceptionText = ex.ToString(),
                    MessageException = ex,
                    Source = ex.Source,
                    StackTrace = ex.StackTrace,
                    Type = NotificationType.Error,
                    UserMessage = "An error occurred while retrieving data. " + query.Query

                });
            }

            return result;
        }

        /// <summary>
        /// Get one entity
        /// </summary>
        /// <typeparam name="TEntity">Object type</typeparam>
        /// <param name="query">The query used to get the object</param>
        /// <param name="timeout"></param>
        /// <returns>The first object returned from the query. If no results are returned, an empty entity is returned.</returns>
        public TEntity GetOneEntity<TEntity>(SqlQuery query, int timeout = 30) where TEntity : class, new()
        {

            var items = new List<TEntity>();

            try
            {
                using (var cmd = _connection.CreateCommand())
                {

                    BuildCommand(query, cmd, timeout);

                    using (var reader = cmd.ExecuteReader())
                    {

                        items = GetDataEntities<TEntity>(reader, query.ObjectMap).ToList();

                    }

                }
            }
            catch (Exception ex)
            {

                _log.Error("An error occurred GetOneEntity<TEntity>. " + query + " " + ex);

                Messages.Add(new ErrorNotification
                {
                    Id = "GetOneEntity<TEntity>",
                    ExceptionText = ex.ToString(),
                    MessageException = ex,
                    Source = ex.Source,
                    StackTrace = ex.StackTrace,
                    Type = NotificationType.Error,
                    UserMessage = "An error occurred while retrieving data. " + query.Query

                });
            }

            return items.Any() ? items.First() : new TEntity();

        }

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <typeparam name="TEntity">Object type</typeparam>
        /// <param name="query">The query used to get the objects</param>
        /// <param name="timeout"></param>
        /// <returns>An IEnumerable of objects of types specified</returns>
        public IEnumerable<TEntity> GetAll<TEntity>(SqlQuery query, int timeout = 30) where TEntity : class
        {

            var items = new List<TEntity>();
            
            try
            {
                using (var cmd = _connection.CreateCommand())
                {

                    BuildCommand(query, cmd, timeout);

                    using (var reader = cmd.ExecuteReader())
                    {

                        items = GetDataEntities<TEntity>(reader, query.ObjectMap).ToList();

                    }

                }
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred GetAll<TEntity>. " + query + " " + ex);

                Messages.Add(new ErrorNotification
                {
                    Id = "GetAll<TEntity>",
                    ExceptionText = ex.ToString(),
                    MessageException = ex,
                    Source = ex.Source,
                    StackTrace = ex.StackTrace,
                    Type = NotificationType.Error,
                    UserMessage = "An error occurred while retrieving data. " + query.Query
                });
            }

            return items;

        }

        /// <summary>
        /// Gets a data reader to be used by service
        /// </summary>
        /// <param name="query">The query used to create the DataReader</param>
        /// <param name="timeout"></param>
        /// <returns>A DataReader</returns>
        public IDataReader GetDataReader(SqlQuery query, int timeout = 30)
        {
            
            try
            {

                using (var cmd = _connection.CreateCommand())
                {
                    BuildCommand(query, cmd, timeout);
                    return cmd.ExecuteReader();
                }

            }
            catch (Exception ex)
            {
                
                _log.Error("An error occurred while getting the reader. " + ex + " " + query);

                Messages.Add(new ErrorNotification
                {
                    Id = "GetDataReader",
                    ExceptionText = ex.ToString(),
                    MessageException = ex,
                    Source = ex.Source,
                    StackTrace = ex.StackTrace,
                    Type = NotificationType.Error,
                    UserMessage = "An error occurred while retrieving data reader."
                });

            }

            return null;

        }

        /// <summary>
        /// Execture scaler
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public object ExecuteScaler(SqlQuery query)
        {

            try
            {
                using (var cmd = _connection.CreateCommand())
                {
                    BuildCommand(query, cmd);
                    return cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {

                Messages.Add(new ErrorNotification
                {
                    Id = "ExecuteScaler",
                    ExceptionText = ex.ToString(),
                    MessageException = ex,
                    Source = ex.Source,
                    StackTrace = ex.StackTrace,
                    Type = NotificationType.Error,
                    UserMessage = "An error occurred while ExecuteScaler."
                });
                
            }

            return null;

        }

        /// <summary>
        /// Gets a data reader result sets to be used by service
        /// </summary>
        /// <param name="queries">A list of queries to use to get multiple result sets</param>
        /// <param name="timeout"></param>
        /// <returns>A DataReader</returns>
        public IDataReader GetDataReaderResultSets(List<SqlQuery> queries, int timeout = 30)
        {

            var sb = new StringBuilder();
            var count = 0;

            foreach (var q in queries)
            {
                sb.Append(q.Query);

                if (count < queries.Count - 1)
                {
                    sb.Append(";");
                }

                count++;
            }

            var query = new GenericSqlQuery {
                Id = "ResultSets", 
                Query = sb.ToString()
            };

            try
            {

                using (var cmd = _connection.CreateCommand())
                {
                    BuildCommand(query, cmd);
                    //cmd.CommandText = query;
                    return cmd.ExecuteReader();
                }

            }
            catch (Exception ex)
            {

                _log.Error("An error occurred while getting the reader result sets. " + ex + " " + query);

                Messages.Add(new ErrorNotification
                {
                    Id = "GetDataReaderResultSets",
                    ExceptionText = ex.ToString(),
                    MessageException = ex,
                    Source = ex.Source,
                    StackTrace = ex.StackTrace,
                    Type = NotificationType.Error,
                    UserMessage = "An error occurred while retrieving data reader."
                });

            }

            return null;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public IDbDataAdapter GetDataAdapter(SqlQuery query, int timeout = 30)
        {

            try
            {

                using (var cmd = _connection.CreateCommand())
                {

                    BuildCommand(query, cmd, timeout);
                    var adapter = _context.Adapter;
                    adapter.SelectCommand = cmd;
                    return adapter;

                }

            }
            catch (Exception ex)
            {

                _log.Error("An error occurred while getting the data adapter. " + ex + " " + query);

                Messages.Add(new ErrorNotification
                {
                    Id = "GetDataAdapter",
                    ExceptionText = ex.ToString(),
                    MessageException = ex,
                    Source = ex.Source,
                    StackTrace = ex.StackTrace,
                    Type = NotificationType.Error,
                    UserMessage = "An error occurred while retrieving data adapter."
                });

            }

            return null;

        }

        /// <summary>
        /// Get DataReaderColumns
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="reader"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public List<DataReaderColumn> GetDataReaderColumns<TEntity>(IDataReader reader, Dictionary<string, string> map) where TEntity : class
        {

            var columns = new List<DataReaderColumn>();

            // Loop through each of the fields in the data reader
            for (var i = 0; i < reader.FieldCount; i++)
            {
                // get the column name
                var dbColName = reader.GetName(i).ToUpper();
                // get the ordinal
                var dbColOrdinal = i;

                // use the map to get the field name for the current database column
                string fieldName;

                if (map.TryGetValue(dbColName, out fieldName))
                {
                    // get the property reference to the field field
                    PropertyInfo pInfo = typeof(TEntity).GetProperty(fieldName);
                    // create the new column from the database and property info
                    var drc = new DataReaderColumn(dbColName, dbColOrdinal, pInfo);

                    // add the column to the list
                    columns.Add(drc);
                }

            }

            return columns;
        }

        /// <summary>
        /// Get entities from data reader using mapping of columns
        /// </summary>
        /// <param name="reader">The data reader</param>
        /// <param name="map">Dictionary map of database column name -> attribute</param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetDataEntities<TEntity>(IDataReader reader, Dictionary<string, string> map) where TEntity : class
        {

            var items = new List<TEntity>();
            var rowCount = 0;
            List<DataReaderColumn> columns = null;

            while (reader.Read())
            {
                // increment the row counter
                rowCount++;

                //on the first row only
                if (rowCount == 1)
                {
                    //get the columns using the reader and map
                    columns = GetDataReaderColumns<TEntity>(reader, map);
                }

                // Create a new instance of the underlying object type
                var item = (TEntity)Activator.CreateInstance(typeof(TEntity));

                // Loop through each of the columns
                if (columns != null)
                {

                    foreach (DataReaderColumn column in columns)
                    {

                        // get the value as a string
                        var stringValue = reader[column.DbColOrdinal].ToString();

                        if (column.PropertyInfo != null && !string.IsNullOrEmpty(stringValue))
                        {
                            // for now, convert nullable types to their underlying types
                            // later, add the ability to retrieve nullable types
                            var fieldType = Nullable.GetUnderlyingType(column.PropertyInfo.PropertyType) ??
                                             column.PropertyInfo.PropertyType;

                            if (fieldType == typeof (int))
                            {
                                column.PropertyInfo.SetValue(item, GetInt(stringValue), null);
                            }
                            else if (fieldType == typeof (double))
                            {
                                column.PropertyInfo.SetValue(item, GetDouble(stringValue), null);
                            }
                            else if (fieldType == typeof (DateTime))
                            {
                                column.PropertyInfo.SetValue(item, GetDateTime(stringValue), null);
                            }
                            else if (fieldType == typeof (char))
                            {
                                if (stringValue.Length > 0)
                                {
                                    column.PropertyInfo.SetValue(item, stringValue[0], null);
                                }
                            }
                            else if (fieldType == typeof (bool))
                            {
                                column.PropertyInfo.SetValue(item, GetBoolean(stringValue));
                            }
                            else
                            {
                                // default to a string
                                column.PropertyInfo.SetValue(item, stringValue, null);
                            }

                        }

                    }

                }

                items.Add(item);
            }

            return items;
        }

        /// <summary>
        /// Get a booean from a string
        /// </summary>
        /// <param name="value">Value to evaluate</param>
        /// <returns>A boolean</returns>
        public bool GetBoolean(string value)
        {
            return value.Equals("Y", StringComparison.OrdinalIgnoreCase)
                || value.Equals("1", StringComparison.OrdinalIgnoreCase)
                || value.Equals("TRUE", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Get an int from string
        /// </summary>
        /// <param name="value">Value to be parsed</param>
        /// <returns>If can parse returns value else returns 0</returns>
        public int GetInt(string value)
        {

            int r;

            bool isConverted = Int32.TryParse(value, out r);

            if (!isConverted)
            {
                _log.Error("Error while trying convert " + value + " to an integer.");
            }

            return r;

        }

        /// <summary>
        /// Get Double from a string
        /// </summary>
        /// <param name="value">Value to be parsed</param>
        /// <returns>If parsed returns the value else returns 0.0</returns>
        public double GetDouble(string value)
        {

            double r;

            bool isConverted = Double.TryParse(value, out r);

            if (!isConverted)
            {
                _log.Error("Error while trying convert " + value + " to a double.");
            }

            return r;

        }

        /// <summary>
        /// Get the DateTime from a string. Will return current DateTime if parse fails.
        /// </summary>
        /// <param name="value">Value to be parsed</param>
        /// <returns>If prased returns DateTime value else returns DateTime.Now</returns>
        public DateTime GetDateTime(string value)
        {

            DateTime d;

            var isConverted = DateTime.TryParse(value, out d);

            if (!isConverted)
            {
                _log.Error("Error while trying convert " + value + " to a DateTime.");
            }

            return d;

        }

        /// <summary>
        /// Add single quotes to elements of a comma delimited string
        /// </summary>
        /// <param name="value">Comma delimited string to have single quotes added to it</param>
        /// <returns>String with single quotes added</returns>
        public string AddSingleQuotes(string value)
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
        public string AddSingleQuotes(List<string> list)
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

        /// <summary>
        /// Returns a DataTable from a data reader
        /// </summary>
        /// <param name="reader">DataReader that contains data for a DataTable</param>
        /// <returns>DataTable containing DataReader data</returns>
        public DataTable GetDataTable(IDataReader reader)
        {

            var table = new DataTable();

            if (reader != null)
            {
                table.Load(reader);
            }

            return table;

        }

        /// <summary>
        /// Add parameters to a command
        /// </summary>
        /// <param name="command">DbCommand to add parameters to</param>
        /// <param name="parameterName">Name of parameter</param>
        /// <param name="parameterValue">Value of the parameter</param>
        /// <param name="type">Type of parameter</param>
        public void AddParameterWithValue(IDbCommand command, string parameterName, object parameterValue, DbType type = DbType.String)
        {

            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;
            parameter.DbType = type;
            command.Parameters.Add(parameter);

        }

        /// <summary>
        /// Build the DbCommand
        /// </summary>
        /// <param name="query">A sql query</param>
        /// <param name="command">DbCommand that needs command text and parameters</param>
        /// <param name="timeout"></param>
        private void BuildCommand(ISqlQuery query, IDbCommand command, int timeout = 30)
        {
            
            if (command == null) return;

            //TODO probably better way to do this but works - Scott Collier 11/5/2014

            if (command.GetType().GetProperty("BindByName") != null)
            {
                command.GetType().GetProperty("BindByName").SetValue(command, true, null);
            }

            command.CommandText = query.Query;

            //Prevent user from setting to 0 which could lead to infinite wait
            if (timeout > 0)
            {
                command.CommandTimeout = timeout;
            }

            if (query.Parameters == null) return;

            foreach (var param in query.Parameters)
            {
                
                if (param.DbType == DbType.String)
                {
                    AddParameterWithValue(command, param.ParameterName, param.Value);
                }
                else
                {
                    AddParameterWithValue(command, param.ParameterName, param.Value, param.DbType);
                }

            }

            command.CommandType = CommandType.Text;

        }

        #region Implementation of Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes resources used.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {

            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                //_log.Debug("Closing Generic Connection");
                _connection.Close();
            }

            if (_context != null)
            {
                _context.Dispose();
            }

            _disposed = true;

        }
        #endregion

    }

}
