namespace verint_service.Models.Config
{
    public class VerintConnectionConfiguration
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string AuthConnectionString { get; set; }

        public string VerintBaseConnectionString { get; set; }

        public string VerintOnlineFormBaseConnectionString { get; set; }
    }
}
