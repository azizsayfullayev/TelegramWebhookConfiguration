namespace FastApiWebhook.Models
{
    public class User
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;

    }
}
