namespace FastApiWebhook.Extentions
{
    public class GetTime
    {
        public static DateTime GetUzbekistanTime()
        {
            // Get the time zone information for Uzbekistan Standard Time (UTC+5)
            TimeZoneInfo uzbekistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tashkent");

            // Get the current UTC time
            DateTime utcNow = DateTime.UtcNow;

            // Convert the UTC time to Uzbekistan local time
            DateTime uzbekistanTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, uzbekistanTimeZone);

            return uzbekistanTime;
        }
    }
}
