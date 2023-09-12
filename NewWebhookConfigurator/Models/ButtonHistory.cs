using FastApiWebhook.Extentions;
using MongoDB.Bson;

namespace FastApiWebhook.Models
{
    public class ButtonHistory
    {
        public ObjectId Id { get; set; }
        public long ChatId { get; set; }
        public string Data { get; set; } = string.Empty;
        public DateTime ClickedTime { get; set; } = GetTime.GetUzbekistanTime();

        
    }
}
