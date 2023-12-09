using System.Diagnostics;

namespace OctopusEnergyBillViewer.Model;

public static class Paths
{
    private static readonly string processName_ = Process.GetCurrentProcess().ProcessName;
    public static string DataDir
    {
        get
        {
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), processName_);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }
    }
}
