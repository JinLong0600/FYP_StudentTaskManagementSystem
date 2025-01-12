
namespace StudentTaskManagement.ViewModels
{
    public class NotificationMessage
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsAggregated { get; set; }
        public string Icon { get; set; }

        public string RequireInteraction { get; set; }

        public string data { get; set; }
        public Dictionary<string, string>? Data { get; internal set; }
    }
}
