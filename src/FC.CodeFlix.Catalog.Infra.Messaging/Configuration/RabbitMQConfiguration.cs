namespace FC.CodeFlix.Catalog.Infra.Messaging.Configuration
{
    public class RabbitMQConfiguration
    {
        public const string CONFIGURATION_SECTION = "RabbitMQ";

        public string? Hostname { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? Exchange { get; set; }
    }
}
