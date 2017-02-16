using System.Runtime.Serialization;
using CalNotify.Models.User;
using Microsoft.AspNetCore.Identity;

namespace CalNotify.Models.Admins
{
    /// <summary>
    /// </summary>
    [DataContract]
    public class WebAdmin : GenericUser
    {
        public WebAdmin() {}

        public WebAdmin(string name, string email , string password)
        {
            this.Name = name;
            this.Email = email;
            // Identity Related
            UserName = email;


            SetPassword(password);
        }


        public void Update(string name, string email, string password = null, string passwordCheck = null)
        {
            if(!string.IsNullOrWhiteSpace(name))
                Name = name;
            if (!string.IsNullOrWhiteSpace(email))
                Email = email;

            if (string.IsNullOrWhiteSpace(passwordCheck) || string.IsNullOrWhiteSpace(password)) return;

            var check = VerifyPassowrd(passwordCheck);
            if (check == PasswordVerificationResult.Success)
            {
                SetPassword(password);
            }
        }

    }
}