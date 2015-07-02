using System;

namespace Infrastructure.Data.Notify
{
    /// <summary>
    /// Represents an error
    /// </summary>
    public class ErrorNotification : Notification
    {

        public Exception MessageException { get; set; }
        public string ExceptionText { get; set; }
        public string StackTrace { get; set; }

        public ErrorNotification()
        {
            MessageException = new Exception();
            Type = NotificationType.Error;
        }

    }

}
