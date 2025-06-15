using Abp.UI;
using CloudinaryDotNet;

namespace TestABP.Configuration
{
    public class CloudinaryConfig
    {
        public string CloudName { get; set; }
        public string APIKey { get; set; }
        public string APISecret { get; set; }

        public Account CloudinaryAccountConfig()
        {
            var cloudName = CloudName;
            var apiKey = APIKey;
            var apiSecret = APISecret;

            if (string.IsNullOrWhiteSpace(cloudName) ||
                string.IsNullOrWhiteSpace(apiKey) ||
                string.IsNullOrWhiteSpace(apiSecret))
            {
                throw new UserFriendlyException("Cloudinary configuration is missing. Please configure in settings.");
            }

            var account = new Account(cloudName, apiKey, apiSecret);

            return account;
        }
    }
}
