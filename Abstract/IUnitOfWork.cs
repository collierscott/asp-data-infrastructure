using System;
using System.Data;

namespace Infrastructure.Data.Abstract
{

    public interface IUnitOfWork : IDisposable
    {

        void BeginTransaction();
        void BeginTransaction(IsolationLevel isolationLevel);
        void Commit();
        void CommitTransaction();
        void RollBackTransaction();

    }

}
