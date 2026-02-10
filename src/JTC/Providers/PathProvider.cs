namespace JTC.Providers
{
    public static class PathProvider
    {
        public static string GetLogPath()
        {
            string directory = OperatingSystem.IsLinux()
                ? "/var/log/jtc"
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "jtc", "logs");
            Directory.CreateDirectory(directory);
            return Path.Combine(directory, "log.txt");
        }

        public static string GetDatabasePath()
        {
            string directory = OperatingSystem.IsLinux()
                ? "/var/lib/jtc"
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "jtc", "data");
            Directory.CreateDirectory(directory);
            return Path.Combine(directory, "jtc.db");
        }
    }
}
