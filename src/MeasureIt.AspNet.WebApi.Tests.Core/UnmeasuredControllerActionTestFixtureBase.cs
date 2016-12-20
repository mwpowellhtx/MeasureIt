using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MeasureIt
{
    using Controllers;
    using Newtonsoft.Json;
    using Xunit;

    public abstract class UnmeasuredControllerActionTestFixtureBase<TStartup> : SelfHostTestFixtureBase<TStartup>
        where TStartup : Startup
    {
        protected UnmeasuredControllerActionTestFixtureBase(string url = null)
            : base(url ?? GetUrl(), "api/unmeasured")
        {
        }

        [Fact]
        public void VerifyApiValuesCorrect()
        {
            // So much of the mechanics of this depends on correct DI container resolution.
            MakeRequest(client => client.GetAsync(BaseApiUrl).Result)
                .Handle(response =>
                {
                    Assert.NotNull(response);

                    const HttpStatusCode ok = HttpStatusCode.OK;

                    Assert.Equal(ok, response.StatusCode);

                    var s = response.Content.ReadAsStringAsync().Result;

                    Assert.NotNull(s);
                    Assert.NotEqual(string.Empty, s);

                    var actualValues = JsonConvert.DeserializeObject<IEnumerable<int>>(s).ToArray();

                    Assert.Equal(UnmeasuredControllerBase.EvenValues, actualValues);
                });
        }

        [Theory
        , InlineData(0, new[] {0})
        , InlineData(1, new int[0])
        , InlineData(2, new[] {2})
        , InlineData(3, new int[0])
        , InlineData(4, new[] {4})
        , InlineData(5, new int[0])
        , InlineData(6, new[] {6})
        , InlineData(7, new int[0])
        , InlineData(8, new[] {8})
        , InlineData(9, new int[0])
        ]
        public void VerifyApiVerifiedValueCorrect(int value, int[] expectedValues)
        {
            // So much of the mechanics of this depends on correct DI container resolution.
            MakeRequest(client => client.GetAsync(string.Format("{0}/verify/{1}", BaseApiUrl, value)).Result)
                .Handle(response =>
                {
                    Assert.NotNull(response);

                    const HttpStatusCode ok = HttpStatusCode.OK;

                    Assert.Equal(ok, response.StatusCode);

                    var s = response.Content.ReadAsStringAsync().Result;

                    Assert.NotNull(s);
                    Assert.NotEqual(string.Empty, s);

                    var actualValues = JsonConvert.DeserializeObject<IEnumerable<int>>(s).ToArray();

                    Assert.Equal(expectedValues, actualValues);
                });
        }
    }
}
