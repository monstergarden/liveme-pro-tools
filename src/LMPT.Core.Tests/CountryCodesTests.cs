using System.Threading.Tasks;
using LMPT.Core.Services;
using NUnit.Framework;
using Shouldly;

namespace Tests
{
    public class CountryCodesTests
    {
        [Test]
        public void  GetFullNameReturnCorrectValueForUS()
        {
            CountryCodes.GetFullName("us").ShouldBe("USA");
        }   
        [Test]
        public async Task TestAsync()
        {
            var res = await LMPT.Core.Services.Helper.Utils.ExtractFirstFrameAsync();
            System.Console.WriteLine(res);
        }   
    }
}