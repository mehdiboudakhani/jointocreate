namespace JTC.Providers
{
    /// <summary>
    /// Provides file paths.
    /// </summary>
    public static class PathProvider
    {
        /// <summary>
        /// Gets the path to the log file and ensures the directory exists.
        /// </summary>
        /// <returns>The path to the log file.</returns>
        public static string GetLogPath()
        {
            string directory = OperatingSystem.IsLinux()
                ? "/var/log/jtc"
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "jtc", "logs");
            Directory.CreateDirectory(directory);
            return Path.Combine(directory, "log.txt");
        }

        /// <summary>
        /// Gets the path to the database file and ensures the directory exists.
        /// </summary>
        /// <returns>The path to the database file.</returns>
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
