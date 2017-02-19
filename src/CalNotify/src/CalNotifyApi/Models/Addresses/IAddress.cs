namespace CalNotifyApi.Models.Addresses
{
    public interface IAddress
    {
        string Street { get; }

        string City { get; }

        string State { get; }
        string Zip { get; }
        IGeoLocation Location { get; }
    }


    public interface IGeoLocation
    {
        double Latitude { get; }
        double Longitude { get; }
    }
}