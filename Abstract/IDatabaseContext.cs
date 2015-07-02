using System;
using System.Data;
using System.Data.Common;
using Infrastructure.Data.Notify;

namespace Infrastructure.Data.Abstract
{

    public interface IDatabaseContext : IDisposable
    {
        Notifications Messages { get; set; }
        DbConnection Connection { get; }
        IDbDataAdapter Adapter { get; }
    }

}
