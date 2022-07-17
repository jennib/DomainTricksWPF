using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DomainTricks_WPF.Models
{
    public class AuthenticationModel
    {
        public AuthenticationModel(string? domainName, string? userName, string? password)
        {
            DomainName = domainName;
            UserName = userName;
            Password = password;
        }

        public string? DomainName  { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        
        public SecureString SecurePassword {get
            {
                SecureString securePassword = new();
                if (Password is not null)
                {
                    foreach (char c in Password)
                    {
                        securePassword.AppendChar(c);
                    }
                }
                return securePassword;
            }
            
        }
    }
}
