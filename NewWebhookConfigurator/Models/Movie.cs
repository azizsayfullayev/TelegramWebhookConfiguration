using FastApiWebhook.Extentions;

namespace FastApiWebhook.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string VideoId { get; set; } = string.Empty;
        public string ThumbnailId { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public string DescriptionEntities { get; set; } = string.Empty;
        public DateTime DateTime { get; set; } = GetTime.GetUzbekistanTime();
        public long AdminId { get; set; }



    }
}
