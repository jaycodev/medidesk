using System.Configuration;

namespace medical_appointment_system.Models
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }

        public static CloudinarySettings FromConfig()
        {
            return new CloudinarySettings
            {
                CloudName = ConfigurationManager.AppSettings["CloudinaryCloudName"],
                ApiKey = ConfigurationManager.AppSettings["CloudinaryApiKey"],
                ApiSecret = ConfigurationManager.AppSettings["CloudinaryApiSecret"]
            };
        }
    }
}