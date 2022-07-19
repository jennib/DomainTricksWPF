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
        Log.Information("SearchDirectoryTask start");

        //https://stackoverflow.com/questions/62180766/async-active-directory-querying
        //     return Task.Run(() =>
        //    {
        DirectoryEntry entry = new(domainPath);
        DirectorySearcher mySearcher = new(entry)
        {
            Filter = filter,
            SizeLimit = int.MaxValue,
            PageSize = int.MaxValue,
            Asynchronous = true
        };
        mySearcher.PropertiesToLoad.AddRange(propertiesToReturn);
        //foreach (string property in propertiesToReturn)
        //{
        //    mySearcher.PropertiesToLoad.Add(property);
        //}
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
            // var it = await  Task.Run(() =>

            // mySearchResults = mySearcher.FindAll();
            // );
            //  mySearchResults = it;
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
        string ret = string.Empty;

        if (sr.Properties[propertyName].Count > 0)
            ret = sr.Properties[propertyName][0].ToString();

        return ret;
    }
}
