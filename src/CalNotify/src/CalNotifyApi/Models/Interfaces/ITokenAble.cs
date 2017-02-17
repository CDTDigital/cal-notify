namespace CalNotify.Models.Interfaces
{
    public interface ITokenAble 
    {
        string Token { get; set; }
        string Name { get; set; }
        string PhoneNumber { get; set; }
        string Email { get; set; }
    }
}