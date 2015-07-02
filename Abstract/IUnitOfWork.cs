using System;
using System.Data;
using System.Data.Objects;

namespace Infrastructure.Data.Abstract
{

    public interface IUnitOfWork : IDisposable
    {

        void BeginTransaction();
        void BeginTransaction(IsolationLevel isolationLevel);
        void Commit();
        void CommitTransaction();
        void RollBackTransaction();
        void SaveChanges();
        void SaveChanges(SaveOptions saveOptions);

    }

}
