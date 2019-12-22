namespace Bookings.Common.Configuration
{
    public class AzureAdConfiguration
    {
        public string Authority { get; set; }
        public string TenantId { set; get; }
        public string AppRegistrationId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
    }
}