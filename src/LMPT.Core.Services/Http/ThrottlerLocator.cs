using LMPT.Core.Services.Config;

namespace LMPT.Core.Services.Http
{
    public class ThrottlerLocator
    {
        public ThrottlerLocator(CoreSettings settings)
        {
            Foreground = new Throttler(settings.Foreground);
            Background = new Throttler(settings.Background);
        }

        public Throttler Foreground { get; }
        public Throttler Background { get; }
    }
    public class ThrottlerLocator2
    {
        CoreSettings _settings;
        public ThrottlerLocator2(CoreSettings settings)
        {
            this._settings = settings;
        }

        public Throttler Foreground  => new Throttler(_settings.Foreground);
        public Throttler Background  => new Throttler(_settings.Background);
    }
}