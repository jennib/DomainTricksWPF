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
    
    public  Task<SearchResultCollection> ADSearcher(
        string domainPath,
        string filter,
        string[] propertiesToReturn)
    {
        #region Example Call
        // Example call

        //DomainService domainService = new(logger);
        //string domainPath = "LDAP://DC=tuttistudios,DC=com";
        //string filter = ("(&(objectClass=computer)(primaryGroupID=515))");
        //string[] propertiesToReturn = { "dNSHostName", "OU", "distinguishedName" };
        //SearchResultCollection searchResults;

        //try
        //{ 
        //     searchResults = await domainService.ADSearcher(domainPath, filter, propertiesToReturn);
        //} 
        //catch (Exception ex)
        //{
        //    Log.Error(ex, "Exception in TestADSearcher: {0}", ex.Message);
        //    return;
        //}

        //Log.Information($"TestSearcher result {searchResults.Count}");

        //foreach (SearchResult result in searchResults)
        //{
        //    Log.Information($"-result {result.GetPropertyValue("DisplayName")}");
        //    Log.Information($" DistinguisedName = {result.GetPropertyValue("distinguishedname")}");
        //    foreach (DictionaryEntry property in result.Properties)
        //    {
        //        foreach (var val in (property.Value as ResultPropertyValueCollection))
        //        {
        //            Log.Information($"--{property.Key} = {val}");
        //        }
        //    }
        //}
        #endregion Example Call
        
        Log.Information("SearchDirectoryTask start");

        DirectoryEntry entry = new(domainPath);
        DirectorySearcher mySearcher = new(entry)
        {
            Filter = filter,
            SizeLimit = int.MaxValue,
            PageSize = int.MaxValue,
            Asynchronous = true
        };
        mySearcher.PropertiesToLoad.AddRange(propertiesToReturn);
        mySearcher.ClientTimeout = TimeSpan.FromSeconds(5);
        mySearcher.ServerTimeLimit = TimeSpan.FromSeconds(5);
        List<SearchResult> resultList = new();
        
        try
        {
            SearchResultCollection mySearchResults = mySearcher.FindAll();
            foreach (SearchResult result in mySearchResults)
            {
                resultList.Add(result);
            }
            mySearcher.Dispose();
            entry.Dispose();
            Log.Information("SearchDirectoryTask end");

            return Task<SearchResultCollection>.FromResult(mySearchResults);
        }
        catch (Exception ex)
        {
            Log.Information($"Error {ex}");
            mySearcher.Dispose();
            entry.Dispose();
            Log.Information("SearchDirectoryTask end");

            throw;
        }
    }    
}

public static class ADExtensionMethods
{
    public static string GetPropertyValue(this SearchResult sr, string propertyName)
    {
        string? ret = string.Empty;

        if (sr.Properties[propertyName].Count > 0)
            ret = sr.Properties[propertyName][0].ToString();

        return ret;
    }
}
