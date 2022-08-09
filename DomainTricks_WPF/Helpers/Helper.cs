using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DomainTricks_WPF.Helpers;

public class Helper
{
    
    public static bool IsUpper(string s)
    {
        return s.All(char.IsUpper);
    }public static bool IsNumbers(string s)
    {
        return s.All(char.IsNumber);
    }public static bool IsLetter(string s)
    {
        return s.All(char.IsLetter);
    }
    public static string FormatBytes(ulong bytes)
    {
        if (bytes < 0) return string.Empty;
        string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
        int i;
        double dblSByte = bytes;
        for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
        {
            dblSByte = bytes / 1024.0;

        }

        return string.Format("{0:0} {1}", dblSByte, Suffix[i]);
    }

    public static void SetMouseCursorToWait()
    {
        // Call from UI thread to avoid an exception.
        Application.Current.Dispatcher.Invoke((Action)delegate
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
        });
    }
    public static void SetMouseCursorToNormal()
    {
        // Call from UI thread to avoid an exception.
        Application.Current.Dispatcher.Invoke((Action)delegate
        {
            Mouse.OverrideCursor = null;  // Restore to default cursor.
        });
    }
}
