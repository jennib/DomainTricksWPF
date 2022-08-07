using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DomainTricks_WPF.Models;

public class AuthenticationModel
{
    public AuthenticationModel(string? domainName, string? userName, string? password,bool runAsLocalUser)
    {
        DomainName = domainName;
        UserName = userName;
        Password = password;
        RunAsLocalUser = runAsLocalUser;
    }
    
    public AuthenticationModel()
    {

    }

    public string? DomainName { get; set; } = string.Empty;
    public string? UserName { get; set; } = String.Empty;
    public string? Password { get; set; } = String.Empty;
    public bool RunAsLocalUser { get; set; } = true;

    public bool IsComplete
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(DomainName)
                && !string.IsNullOrWhiteSpace(UserName)
                && !string.IsNullOrWhiteSpace(Password))
            {
                return false;
            }
            return false;
        }
    }

    public void UpdateFromDisk()
    {
        RunAsLocalUser = Properties.Settings.Default.RunAsLocalUser;
        DomainName = Properties.Settings.Default.DomainName;
        UserName = Properties.Settings.Default.UserName;

        string? tempPassword = Properties.Settings.Default.Password;
        if (string.IsNullOrWhiteSpace(tempPassword))
        {
            Password = string.Empty;
            // TODO: Ask user for password.
        }
        else
        {
            Password = tempPassword;
        }
    }

    public SecureString SecurePassword
    {
        get
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
