using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace LMPT.Core.Server.Utils
{
    public class JsInteropHelper
    {
        private readonly IJSRuntime _jsRuntime;

        public JsInteropHelper(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<List<T>> CallAndGetArray<T>(string function, params object[] parameter)
        {
            var res = await _jsRuntime.InvokeAsync<object[]>(
                function, parameter);

            return res.Select(o =>
            {
                var json = JsonConvert.SerializeObject(o);
                return JsonConvert.DeserializeObject<T>(json);
            }).ToList();
        }

        public async Task<T> CallAndGet<T>(string function, params object[] parameter)
        {
            var res = await _jsRuntime.InvokeAsync<object>(
                function, parameter);

            var json = JsonConvert.SerializeObject(res);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task OpenBookmarks()
        {
            await _jsRuntime.InvokeAsync<object>("opppenBookmarks");
        }
        public async Task OpenUrlInNewTab(string url)
        {
            await _jsRuntime.InvokeAsync<string>("openURL", url);
        }

        public async Task QuitFrontEnd()
        {
            await _jsRuntime.InvokeAsync<object>("quit");
        }
    }
}