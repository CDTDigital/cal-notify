using System.Runtime.Serialization;
using Microsoft.AspNetCore.Identity;

namespace CalNotifyApi.Models.Admins
{
    /// <summary>
    /// </summary>
    [DataContract]
    public class WebAdmin : BaseUser
    {
        public WebAdmin() {}

        public WebAdmin(string name, string email , string password)
        {
            this.Name = name;
            this.Email = email;
            // Identity Related
            UserName = email.ToLower();

            // Always start enabled 
            Enabled = true;
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