namespace StudentTaskManagement.Models
{
    public class PushSubscriptions
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Endpoint { get; set; }
        public string P256dh { get; set; }
        public string Auth { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
