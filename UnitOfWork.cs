using System;
using System.Data;
using System.Data.Objects;
using Infrastructure.Data.Abstract;
using log4net;

namespace Infrastructure.Data
{
    //TODO This is in progress - Scott Collier 10/28/2014
    public class UnitOfWork : IUnitOfWork
    {

        private readonly IDatabaseContext _context;
        // ReSharper disable once InconsistentNaming
        private readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool _disposed;

        private IDbTransaction _transaction;

        public UnitOfWork(IDatabaseContext context)
        {
            _context = context;
            
        }

        public bool IsInTransaction
        {
            get { return _transaction != null; }
        }

        public void BeginTransaction()
        {
            BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            if (_transaction != null)
            {
                _log.Error(@"Cannot begin a new transaction while an existing transaction is still running. 
                            Please commit or rollback the existing transaction before starting a new one.");
            }

            OpenConnection();
            _transaction = _context.Connection.BeginTransaction(isolationLevel);
        }

        public void Commit()
        {

            if (_transaction != null)
            {

                try
                {
                    _transaction.Commit();
                }
                catch (Exception ex)
                {
                    _log.Error("An error occurred committing the transaction. " + ex);
                    _transaction.Rollback();
                }

                _transaction.Dispose();
                _transaction = null;

            }
            else
            {
                _log.Error("An error occurred committing the transaction. Transaction could not be opened.");
            }

        }

        public void RollBackTransaction()
        {
            if (_transaction == null)
            {
                throw new ApplicationException("Cannot roll back a transaction while there is no transaction running.");
            }

            if (IsInTransaction)
            {
                _transaction.Rollback();
                ReleaseCurrentTransaction();
            }
        }

        public void CommitTransaction()
        {
            if (_transaction == null)
            {
                throw new ApplicationException("Cannot roll back a transaction while there is no transaction running.");
            }

            try
            {
                _transaction.Commit();
                ReleaseCurrentTransaction();
            }
            catch
            {
                RollBackTransaction();
                throw;
            }
        }

        public void SaveChanges()
        {
            if (IsInTransaction)
            {
                throw new ApplicationException("A transaction is running. Call CommitTransaction instead.");
            }
            //_context.BuildObjectContext().SaveChanges();
        }

        public void SaveChanges(SaveOptions saveOptions)
        {
            if (IsInTransaction)
            {
                throw new ApplicationException("A transaction is running. Call CommitTransaction instead.");
            }

            //_context.BuildObjectContext().SaveChanges(saveOptions);
        }

        private void ReleaseCurrentTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        private void OpenConnection()
        {
            if (_context.Connection.State != ConnectionState.Open)
            {
                _context.Connection.Open();
            }
        }

        #region dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes off the managed and unmanaged resources used.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (_disposed)
                return;

            _disposed = true;
        }
        #endregion

    }

}
