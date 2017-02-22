using Microsoft.AspNetCore.Mvc;

namespace CalNotifyApi.Events.Exceptions
{
    public interface IProcessEventException
    {
        IActionResult ResponseShellError { get; }
        string Message { get; }
    }
}
