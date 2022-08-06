using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DomainTricks_WPF.Models;

public class AuthenticationModel
{
    public AuthenticationModel(string? domainName, string? userName, string? password)
    {
        DomainName = domainName;
        UserName = userName;
        Password = password;
    }
    public AuthenticationModel()
    {

    }

    public string? DomainName { get; set; } = string.Empty;
    public string? UserName { get; set; } = String.Empty;
    public string? Password { get; set; } = String.Empty;
    
    public bool IsComplete {
        get
        {
            if (DomainName is not null && UserName is not null && Password is not null)
            {
                if (!string.IsNullOrEmpty(DomainName)  && !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                {
                    return false;
                }
            }
            return false;
        }
    }

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
