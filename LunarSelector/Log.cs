namespace LunarSelector
{
    internal static class Log
    {
        private static TextWriter? logfileStream = null;

        internal enum Level { INF, WRN, ERR }

        internal static void Info(string message)
        {
            Write(message, Level.INF);
        }

        internal static void Error(string message)
        {
            Write(message, Level.ERR);
        }

        internal static void Write(string message, Level level)
        {
            Console.WriteLine(message);
            logfileStream ??= new StreamWriter("./log.txt", true);
            logfileStream.WriteLine($"{DateTime.Now:yyyy-MM-dd hh:mm:ss} [{level}] " + message);
            logfileStream.Flush();
        }
    }
}
