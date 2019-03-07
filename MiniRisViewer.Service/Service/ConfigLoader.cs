namespace MiniRisViewer.Domain.Service
{
    public class ConfigLoader
    {
        public static string ReadRegistry(string serviceName)
        {
            using (var regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\UsFileImporter"))
            {
                return (string)regkey.GetValue("ImagePath");
            }
        }
    }
}