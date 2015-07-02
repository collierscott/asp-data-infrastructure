using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Infrastructure.Data.Notify
{

    /// <summary>
    /// A collection of Notifications
    /// </summary>
    public class Notifications : List<Notification>
    {

        // ReSharper disable once UnusedMember.Local
        private readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Success messages
        /// </summary>
        public IEnumerable<Notification> SuccessMessages
        {
            get
            {
                var results = this.Where(m => m.Type == NotificationType.Success);

                return results.ToList();

            }

        }

        /// <summary>
        /// Error messages
        /// </summary>
        public IEnumerable<ErrorNotification> ErrorMessages
        {

            get
            {
                var results = this.Where(m => m.Type == NotificationType.Error);
                return results.ToList().Cast<ErrorNotification>().ToList();
            }

        }

        /// <summary>
        /// Informational messages
        /// </summary>
        public IEnumerable<Notification> InformationMessages
        {
            get
            {
                var results = this.Where(m => m.Type == NotificationType.Information);
                return results.ToList();
            }
        }

        /// <summary>
        /// Queries
        /// </summary>
        public IEnumerable<Notification> QueryMessages
        {
            get
            {
                var results = this.Where(m => m.Type == NotificationType.Query);
                return results.ToList();
            }
        }

        /// <summary>
        /// Warnings
        /// </summary>
        public IEnumerable<Notification> WarningMessages
        {
            get
            {
                var results = this.Where(m => m.Type == NotificationType.Warning);
                return results.ToList();
            }
        }

        /// <summary>
        /// Formats the Notification into a user readable string
        /// </summary>
        /// <returns>String representation of the Notification</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var item in this)
            {
                sb.AppendLine();
                sb.AppendLine("ID: " + item.Id);
                sb.AppendLine("Type: " + item.Type);
                sb.AppendLine("Source: " + item.Source);

                sb.AppendLine("User Message: " + item.UserMessage);

                if (item.Type == NotificationType.Error)
                {
                    var error = (ErrorNotification)item;

                    sb.AppendLine("Text: " + error.ExceptionText);

                    if (error.MessageException != null)
                    {
                        sb.AppendLine("Stack Trace: " + error.StackTrace);
                        sb.AppendLine("Exception: " + error.MessageException);
                    }

                }

                sb.AppendLine();
                sb.AppendLine();
            }


            return sb.ToString();

        }

    }

}