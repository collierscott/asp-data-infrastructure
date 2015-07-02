using System.Configuration;
using System.Data;
using System.Data.Common;
using Infrastructure.Data.Abstract;
using Infrastructure.Data.Notify;
using System;
using log4net;
using StackExchange.Profiling;

namespace Infrastructure.Data
{

    public class DatabaseContext : IDatabaseContext
    {

        // ReSharper disable once InconsistentNaming
        private readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ReSharper disable once NotAccessedField.Local
        private bool _disposed;
        private readonly string _connectionString;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly string _name;
        private readonly DbProviderFactory _factory;
        private DbConnection _connection;
        //private bool _recreateDatabaseIfExists;
        //private bool _lazyLoadingEnabled;

        public Notifications Messages { get; set; }

        public DatabaseContext()
        {

        }

        public DatabaseContext(string connectionName)
        {
            
            var settings = ConfigurationManager.ConnectionStrings[connectionName];

            if (settings == null)
            {
                _log.Error(string.Format("Failed to find a connection string named '{0}' in web.config.", connectionName));
            }
            else
            {

                _name = settings.ProviderName;
                _factory = DbProviderFactories.GetFactory(_name);
                _connectionString = settings.ConnectionString;

            }

            Messages = new Notifications();
            
        }

        /// <summary>
        /// Returns an open connection if possible
        /// </summary>
        public DbConnection Connection
        {

            get
            {

                if (_connection == null)
                {

                    //If in Debug mode, then 
                    #if (DEBUG)
                        _connection = new StackExchange.Profiling.Data.ProfiledDbConnection(_factory.CreateConnection(), MiniProfiler.Current);
                    #else
                        _connection = _factory.CreateConnection();
                    #endif
                    
                    if (_connection != null)
                    {
                        _connection.ConnectionString = _connectionString;
                    }
                    else
                    {
                        _log.Error(string.Format("Connection for '{0}' is null.", _connectionString));
                        return _connection;
                    }

                }

                if (_connection.State != ConnectionState.Open)
                {

                    try
                    {
                        //_log.Debug("Opening Connection");
                        _connection.Open();
                    }
                    catch (Exception ex)
                    {

                        _log.Error(string.Format("Connection for '{0}' could not be opened. " + ex, _connectionString));
                        
                        Messages.Add(new ErrorNotification
                        {
                            Id = "OpenConnectionError",
                            ExceptionText = ex.ToString(),
                            MessageException = ex,
                            Source = ex.Source,
                            StackTrace = ex.StackTrace,
                            Type = NotificationType.Error,
                            UserMessage = "An error occurred while trying to open a database connection."

                        });

                    }
                    
                }

                return _connection;

            }

        }

        public IDbDataAdapter Adapter
        {
            get { return _factory.CreateDataAdapter(); }
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
        protected void Dispose(bool disposing)
        {

            //_log.Debug("Disposing the DatabaseContext");

            //The connection should already be closed at this point.
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                //_log.Debug("Closing Context Connection");
                _connection.Close();
            }
            
            _disposed = true;
        }
        #endregion

    }

}
