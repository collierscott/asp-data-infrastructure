using Infrastructure.Data.Notify;

namespace Infrastructure.Data
{
    public class DataServiceResult<T>
    {

        public string Id { get; set; }
        public T Result { get; set; }
        public Notifications Messages { get; set; }

        /// <summary>
        /// A result from a service. This can be used to return the result plus any notifications associated
        /// with the result
        /// </summary>
        public DataServiceResult()
        {

            Messages = new Notifications();
            Result = default(T);

        }

    }

}
