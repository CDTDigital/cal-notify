namespace CalNotify.Models.Interfaces
{
    public interface ITokenAble : ISmsAble
    {
        string Token { get; set; }
    }
}