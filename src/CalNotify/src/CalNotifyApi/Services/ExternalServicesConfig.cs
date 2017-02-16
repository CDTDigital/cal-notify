namespace CalNotify.Services
{
    public class ExternalServicesConfig
    {
        public GeocodioConfig Geocodio { get; set; }
        public TwilloConfig Twillo { get; set; }
        public GoogleConfig Google { get; set; }
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
  
}
