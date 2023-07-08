using System.Text.Json;

namespace LunarSelector
{
    internal static class Config
    {
        private class InternalConfig
        {
            public bool DownloadLM { get; set; } = false;
            public bool CloseAutomatically { get; set; } = false;

            public static InternalConfig FromFileOrDefault(string file)
            {
                if (!File.Exists(file))
                {
                    return new InternalConfig();
                }

                using var fs = File.OpenRead(file);
                return JsonSerializer.Deserialize<InternalConfig>(fs) ?? new InternalConfig();
            }
        }

        private static InternalConfig? _config = null;
        private static InternalConfig internalConfig => _config ??= InternalConfig.FromFileOrDefault("./config.json");


        public static bool DownloadLM => internalConfig.DownloadLM;
        public static bool CloseAutomatically => internalConfig.CloseAutomatically;
    }
}
