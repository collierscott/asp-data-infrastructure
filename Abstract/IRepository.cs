using System;
using Infrastructure.Data.Notify;
using System.Collections.Generic;
using System.Data;

namespace Infrastructure.Data.Abstract
{

    public interface IRepository : IDisposable
    {

        int Insert<TEntity>(TEntity entity, SqlQuery query, int timeout = 30) where TEntity : class;
        int Update<TEntity>(TEntity entity, SqlQuery query, int timeout = 30) where TEntity : class;
        int Delete<TEntity>(string id, SqlQuery query, int timeout = 30) where TEntity : class;

        int Insert(SqlQuery query, int timeout = 30);
        int Update(SqlQuery query, int timeout = 30);
        int Delete(string id, SqlQuery query, int timeout = 30);

        TEntity GetOneEntity<TEntity>(SqlQuery query, int timeout = 30) where TEntity : class, new();
        IEnumerable<TEntity> GetAll<TEntity>(SqlQuery query, int timeout = 30) where TEntity : class;

        IDataReader GetDataReader(SqlQuery query, int timeout = 30);
        object ExecuteScaler(SqlQuery query);
        IDataReader GetDataReaderResultSets(List<SqlQuery> queries, int timeout = 30);
        DataTable GetDataTable(IDataReader reader);

        Notifications Messages { get; set; }
        IUnitOfWork UnitOfWork { get; }
        int ExecuteStoredProcedure(string name, SqlQuery query);

    }

}
