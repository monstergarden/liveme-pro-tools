using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Hosting;

namespace LMPT.Core.Server
{
    public class Interactive<T>
    {
        private ScriptState state;
        private object _lock = new object();
        private readonly T globals;

        public Interactive(T globals)
        {
            this.globals = globals;
        }

        

        public async Task Init()
        {
            state = await InitStateAsync();
        }

        public async Task<string> Eval(string code)
        {
            try
            {
                state = await state.ContinueWithAsync(code);
                return state.ReturnValue?.ToString() ?? "<empty>";

            }
            catch (System.Exception e)
            {
                return e.Message;
            }
        }

        private async Task<ScriptState> InitStateAsync()
        {
            var options = ScriptOptions.Default
                  .WithReferences(typeof(Program).Assembly)
                  .WithImports(
                      "System.Collections.Generic",
                      "System",
                      "System.Linq"
                      );

            var finalScript = CSharpScript.Create("", options, typeof(T));
            finalScript.Compile();
            ScriptState<object> result = await finalScript.RunAsync(globals);
            return result;
        }


    }
}