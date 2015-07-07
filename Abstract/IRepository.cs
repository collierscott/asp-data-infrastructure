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

        TEntity GetOneEntity<TEntity>(SqlQuery query, int timeout = 30) where TEntity : class, new();
        IEnumerable<TEntity> GetAll<TEntity>(SqlQuery query, int timeout = 30) where TEntity : class;
        IDataReader GetDataReader(SqlQuery query, int timeout = 30);
        IDataReader GetDataReaderResultSets(List<SqlQuery> queries, int timeout = 30);

        string AddSingleQuotes(string value);
        string AddSingleQuotes(List<string> list);

        Notifications Messages { get; set; }

        bool GetBoolean(string value);
        int GetInt(string value);
        double GetDouble(string value);
        DateTime GetDateTime(string value);

        DataTable GetDataTable(IDataReader reader);

    }

}
