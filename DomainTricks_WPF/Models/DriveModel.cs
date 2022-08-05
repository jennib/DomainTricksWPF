using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainTricks_WPF.Helpers;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Generic;


namespace DomainTricks_WPF.Models;

public class DriveModel
{
    private string? _deviceID;
    private ulong? _freeSpace;
    private ulong? _size;

    public string? DeviceID { get => _deviceID; set => _deviceID = value; }
    public UInt64? FreeSpace { get => _freeSpace; set => _freeSpace = value; }
    public UInt64? Size { get => _size; set => _size = value; }

    public string FreeSpaceString
    {
        get { return Helper.FormatBytes((ulong)FreeSpace); }
    }

    public string SizeString
    {
        get { return Helper.FormatBytes((ulong)Size); }
    }

    public double PercentFreeSpace
    {
        get
        {
            double percentFreeSpace = -1;
            if (_size > 0 && _freeSpace > 0)
            {
                double i = (double)((_freeSpace * 1.0) / (_size * 1.0)) * 100;
                percentFreeSpace = Math.Round(i, 1);
            }
            return percentFreeSpace;
        }
    }

    public string PercentColor
    {
        get
        {
            if (PercentFreeSpace <= 10)
            {
                return "LightCoral";
            }
            else
            {
                return "White";
            }
        }

    }
}
