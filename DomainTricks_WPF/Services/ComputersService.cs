using DomainTricks_WPF.Models;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainTricks_WPF.Services
{
    public class ComputersService
    {

        public  ComputersService(ILogger logger)
        {
            Log.Logger = logger;
        }
      

        public async Task<List<ComputerModel>> GetComputers(string domainPath)
        {
            List<ComputerModel> computers = new ();

            // Get a list of Computers from the Directory.
            ADService adService = new ADService(Log.Logger);
            computers = await adService.GetListOfComputersFromADAsync(domainPath);

            return computers;
        }
    }
}
