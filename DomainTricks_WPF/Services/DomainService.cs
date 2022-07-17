using Serilog;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainTricks_WPF.Services
{
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
    }
}
