using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Generic;


namespace DomainTricks_WPF.Models
{
    public class DriveModel 
    {
        public string? DeviceID { get; set; }
        public UInt64? FreeSpace { get; set; }
        public UInt64? Size { get; set; }
    }
}
