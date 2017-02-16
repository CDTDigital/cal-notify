using Microsoft.AspNetCore.Mvc;

namespace CalNotify.Events.Exceptions
{
    public interface IProcessEventException
    {
        IActionResult ResponseShellError { get; }
        string Message { get; }
    }
}
