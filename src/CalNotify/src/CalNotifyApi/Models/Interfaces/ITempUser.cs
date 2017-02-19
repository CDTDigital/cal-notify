namespace CalNotifyApi.Models.Interfaces
{
    public interface ITempUser 
    {
        string Token { get; set; }
        string Name { get; set; }
        string PhoneNumber { get; set; }
        string Email { get; set; }
    }
}