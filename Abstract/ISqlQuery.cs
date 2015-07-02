using System.Collections.Generic;
using Infrastructure.Data.Notify;

namespace Infrastructure.Data.Abstract
{
    public interface ISqlQuery
    {
        string Query { get; set; }
        IList<QueryParameter> Parameters { get; set; }
        Dictionary<string, string> ObjectMap { get; set; }
        Notification Message { get; set; }
    }
}
