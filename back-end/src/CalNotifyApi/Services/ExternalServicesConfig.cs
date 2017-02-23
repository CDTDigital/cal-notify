namespace CalNotifyApi.Services
{
    public class ExternalServicesConfig
    {
        public GeocodioConfig Geocodio { get; set; }
        public TwilloConfig Twillo { get; set; }
        public GoogleConfig Google { get; set; }

        public EmailConfig Email { get; set; }
    }


    public class GoogleConfig
    {    
        public string MapsKey { get; set; }
    }

    public class GeocodioConfig
    {
        public string SecretKey { get; set; }
    }

    public class TwilloConfig
    {
        public string Id { get; set; }

        public string Number { get; set; }
        public string SecretKey { get; set; }
    }

    public class EmailConfig
    {
        public Validation Validation { get; set; }

    }

    public class Validation
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string APIUrl { get; set; }
        public string HelpUrl { get; set; }
        public string SetPasswordUrl { get; set; }
    }
  
}
