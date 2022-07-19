using DomainTricks_WPF.Models;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainTricks_WPF.Services;

public class ADService
{
    // Get a list of Computers from the Directory and return them as ComputerModel 
    public ADService(ILogger logger)
    {
        Log.Logger = logger;
    }

    public async Task<List<ComputerModel>> GetListOfComputersFromADAsync(string domainPath)
    {
        List<ComputerModel> ListOfComputers = new();

        DomainService domainService = new(Log.Logger);
        //string domainPath = "LDAP://DC=tuttistudios,DC=com";
        string filter = ("(&(objectClass=computer)(primaryGroupID=515))");
        string[] propertiesToReturn = { "dNSHostName", "OU", "distinguishedName" };
        SearchResultCollection searchResults;

        try
        {
            searchResults = await domainService.ADSearcher(domainPath, filter, propertiesToReturn);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception in TestADSearcher: {0}", ex.Message);
            throw;
        }

        Log.Information($"TestSearcher result {searchResults.Count}");

        foreach (SearchResult result in searchResults)
        {
            //Log.Information($"-result {result.GetPropertyValue("DisplayName")}");
            Log.Information($" DistinguishedName = {result.GetPropertyValue("distinguishedname")}");
            string distinguishedName = result.GetPropertyValue("distinguishedname");

            // Extract Computer Name and OU from DitinguishedName.
            string[]? componentArray = distinguishedName.Split(',');
            // Computer name.
            string? computerName = Array.Find(componentArray, n => n.StartsWith("CN="));
            computerName = computerName?.Substring(3);
            Log.Information($"Computer name is: {computerName}");
            // OUs
            List<string>? ouList = new();
            foreach (string sub in componentArray)
            {
                if (sub.StartsWith("OU="))
                {
                    ouList.Add(sub.Substring(3));
                }
            }
            ComputerModel computer = new(computerName, Log.Logger);
            computer.OUList = ouList;
            ListOfComputers.Add(computer);
        }

        return ListOfComputers;
    }
}
