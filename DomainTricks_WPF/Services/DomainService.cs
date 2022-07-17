using Serilog;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainTricks_WPF.Services;

public class DomainService
{
    public DomainService(ILogger logger)
    {
        Log.Logger = logger;
    }

    // Returns the current domain e.g. LDAP://DC=tuttistudios,DC=com .
    public static async Task<string> GetCurrentDomainPathAsync()
    {
        DirectoryEntry directoryEntry = new("LDAP://RootDSE");
        try
        {
            return await Task.Run(() =>
            {
                try
                {
                    String? res = "LDAP://" + directoryEntry?.Properties["defaultNamingContext"][0]?.ToString();
                    return res;
                }
                catch (Exception ex)
                {
                    Log.Information($"GetCurrentDomainPathAsync excption {ex.Message}");
                    return "-1";
                }
            });
        }
        catch (Exception ex)
        {
            // This will never be called and I do not know why.
            Log.Information("Error getting domain path: " + ex.Message);
            throw;
        }
    }

    // Returns a domain name from an LDAP path.
    // Takes e.g. LDAP://DC=mydomain,DC=com as input and outputs mydomain.com.
    public static string DomainNameFromLDAPPath(string LDAPString)
    {
        if (LDAPString.Contains("//"))
        {
            string tempName = LDAPString[7..];
            string[] arrayOfStrings = tempName.Split(',');
            for (int i = 0; i < arrayOfStrings.Length; i++)
            {
                if (arrayOfStrings[i].Contains("DC="))
                {
                    arrayOfStrings[i]= arrayOfStrings[i].Substring(3);
                }
            }
           
            string combinedString = string.Join('.', arrayOfStrings);

            return combinedString;
        }
        else
        {
            return LDAPString;
        }
    }
}
