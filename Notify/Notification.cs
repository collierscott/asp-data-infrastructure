namespace Infrastructure.Data.Notify
{
    /// <summary>
    /// A basic notification
    /// </summary>
    public class Notification
    {
        public string Id { get; set; }
        public NotificationType Type { get; set; }
        public string UserMessage { get; set; }
        public string Source { get; set; }
    }
}
