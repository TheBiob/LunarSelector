using LunarSelector;
using System.Diagnostics;
using System.Text;

static void assert(bool condition, string message)
{
    if (!condition)
    {
        Log.Error(message);

        if (!Config.CloseAutomatically)
        {
            Console.WriteLine("Press any key to exit");
            Console.ReadKey(true);
        }
        Environment.Exit(1);
    }
}


// Set current Working Directory to the application's directory, saving the original CWD so LM can be started with it.
var initialWorkingDirectory = Directory.GetCurrentDirectory();
Directory.SetCurrentDirectory(AppContext.BaseDirectory);

string[] extensions = { ".sfc", ".smc" };

assert(args.Length == 1 && File.Exists(args[0]) && extensions.Contains(Path.GetExtension(args[0]).ToLower()), "No ROM provided");

var fs = File.OpenRead(args[0]);
var header = fs.Length % 0x8000;
assert((header == 0 || header == 0x200) && fs.Length >= 0x80000+header, "Invalid ROM size");

var expected = "Lunar Magic Version ";
fs.Position = 0x7F0A0 + header;

var array = new byte[expected.Length];
await fs.ReadAsync(array, 0, array.Length);

var str = Encoding.ASCII.GetString(array);
assert(str.Equals(expected), "ROM does not appear to be LM modified");

array = new byte[4];
await fs.ReadAsync(array, 0, array.Length);
fs.Close();

Directory.CreateDirectory("./LM/");

var version = Encoding.ASCII.GetString(array).Replace('/', '_').Replace('\\', '_');

var lmExe = $"{LMDownloader.LMFolder}LM{version}.exe";
if (!File.Exists(lmExe))
{
    assert(Config.DownloadLM, $"{lmExe} could not be found.");

    Log.Info($"{lmExe} not found. Attempting to download");
    var dl = new LMDownloader(version);
    assert(await dl.ExistsAsync(), "Could not find download link for LM Version " + version);

    assert(await dl.DownloadAsync(), "Failed to download LM Version " + version);
}

Log.Info($"Starting \"{args[0]}\" with LM Version {version}");
Process.Start(new ProcessStartInfo(lmExe, "\"" + args[0] + "\"")
{
    WorkingDirectory = initialWorkingDirectory
});

if (!Config.CloseAutomatically)
{
    Console.WriteLine("Press any key to exit");
    Console.ReadKey(true);
}
