using System.IO;
using Newtonsoft.Json;

namespace LMPT.Core.Services.Config
{
    public class CoreSettings
    {
        public CoreSettings()
        {
        }

        public CoreSettings(DirectoryInfo storeIn)
        {
            StoreIn = storeIn.FullName;
        }

        [JsonIgnore] public int Id { get; set; } = 1;


        public string Email { get; set; }
        public string Password { get; set; }

        public ThrottlerConfig Foreground { get; set; }
        public ThrottlerConfig Background { get; set; }
        public string StoreIn { get; set; }

        public static CoreSettings GetCoreSettings(DirectoryInfo storeIn)
        {
            var file = Path.Combine(storeIn.FullName, "coreSettings.json");
            if (File.Exists(file))
            {
                var raw = File.ReadAllText(file);
                var sett = JsonConvert.DeserializeObject<CoreSettings>(raw);
                return sett;
            }

            var defaultSettings = GetDefaultSettings(storeIn);
            var rawJson = JsonConvert.SerializeObject(defaultSettings, Formatting.Indented);
            File.WriteAllText(file, rawJson);
            return defaultSettings;
        }


        private static CoreSettings GetDefaultSettings(DirectoryInfo storeIn)
        {
            return new CoreSettings(storeIn)
            {
                Foreground = new ThrottlerConfig
                {
                    RandomDelay = new Range(0, 2),
                    MaxConcurrentCalls = 3
                },

                Background = new ThrottlerConfig
                {
                    RandomDelay = new Range(0, 1),
                    MaxConcurrentCalls = 5
                }
            };
        }
    }
}