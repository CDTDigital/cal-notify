using System;
using System.Collections.Generic;
using CalNotify.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CalNotify.Events.Exceptions
{
    /// <summary>
    /// Exception to be used during event related processing if the event can't continue
    /// processing due to some forseen scenario. e.g. when trying to get a customer's card, the initial
    /// validation for a user might succed but a card want found. Or if during a upload of an avatar
    /// </summary>
    public class ProcessEventException : Exception, IProcessEventException
    {

        public ProcessEventException(string msg) : base(msg)
        {
        }

        public ProcessEventException(string msg, List<string> details): base(msg)
        {
            _details = details;
        }

        private readonly List<string> _details;
        public IActionResult ResponseShellError =>  ResponseShell.Error(Message, _details);
    }
}