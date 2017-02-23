﻿using CalNotifyApi.Models.Auth;

namespace CalNotifyApi.Models.Interfaces
{
    public interface ITempUser 
    {
        string Token { get; set; }
        TokenType TokenType { get; set; }
        string Name { get; set; }
        string PhoneNumber { get; set; }
        string Email { get; set; }

       
    }
}