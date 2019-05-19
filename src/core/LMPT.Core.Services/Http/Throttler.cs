using System;
using System.Threading;
using System.Threading.Tasks;
using LMPT.Core.Services.Config;
using Microsoft.VisualStudio.Threading;

namespace LMPT.Core.Services.Http
{
    public class Throttler
    {
        private readonly object _lock = new object();
        private readonly Random _rnd = new Random();
        private AsyncSemaphore _asyncSemaphore;
        private DateTime _lastCall;
        private Range _randomDelay;


        public Throttler(ThrottlerConfig config)
        {
            Config = config;

            _randomDelay = config.RandomDelay;
            _asyncSemaphore = new AsyncSemaphore(config.MaxConcurrentCalls);
        }

        private ThrottlerConfig Config { get; }
        private Guid Guid { get; } = Guid.NewGuid();

        private TimeSpan GetDelay()
        {
            var addToLower = _rnd.NextDouble() * (_randomDelay.Upper - _randomDelay.Lower);
            var delayInSec = _randomDelay.Lower + addToLower;
            return TimeSpan.FromSeconds(delayInSec);
        }


        public void SetConcurrentCalls(int concurrentCalls)
        {
            _asyncSemaphore = new AsyncSemaphore(concurrentCalls);
            Config.MaxConcurrentCalls = concurrentCalls;
        }

        public void SetDelay(int lower, int upper)
        {
            _randomDelay = new Range(lower, upper);
            Config.RandomDelay = _randomDelay;
        }

        public async Task<T> WithLimitedCalls<T>(Func<Task<T>> onClient)
        {
            using (await _asyncSemaphore.EnterAsync(CancellationToken.None).ConfigureAwait(true))
            {
                var timeSpan = GetDelay();

                if ((DateTime.UtcNow - _lastCall).TotalSeconds < 2)
                {
                    Console.WriteLine($"Limited Caller ID: {Guid} With Delay: {timeSpan.TotalSeconds}");
                    await Task.Delay(timeSpan).ConfigureAwait(true);
                }
                else
                {
                    Console.WriteLine($"Limited Caller ID: {Guid} Without delay");
                }


                var res = await onClient().ConfigureAwait(true);
                lock (_lock)
                {
                    _lastCall = DateTime.UtcNow;
                }

                return res;
            }
        }
    }
}