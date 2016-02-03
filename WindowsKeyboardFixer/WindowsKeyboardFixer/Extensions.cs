using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// extensions for the IntPtr inbuild class.
/// </summary>
public static class ExtensionsIntPtr {

    /// <summary>
    /// converts to a hex string (using the 32 bit).
    /// </summary>
    /// <param name="ptr"></param>
    /// <returns></returns>
    public static string convertTo32BitHex(this IntPtr ptr) {
        var val = ptr.ToInt32() & 0x0000ffff;
        return val.ToString("x8");
    }
}
