using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MeasureIt.Castle.Windsor.AspNet.WebApi
{
    using Controllers;
    using Newtonsoft.Json;
    using Xunit;

    public class MeasuredControllerActionTests : SelfHostTestFixtureBase<MeasuredStartupFixture>
    {
        public MeasuredControllerActionTests()
            : base(GetUrl(), "api/measured")
        {
        }

        [Fact]
        public void VerifyApiValuesCorrect()
        {
            /* TODO: TBD: I would like to also verify the counter somehow, but the challenging
             * part is connecting the dots with the Controler side without white-box testing the
             * controller itself. Perhaps there is a way to wire up the DI container behind
             * the scenes, upon instance activation, etc? */

            // If this works, counters included, then there should be no exceptions during the test.
            MakeRequest(client => client.GetAsync(BaseApiUrl).Result)
                .Handle(response =>
                {
                    Assert.NotNull(response);

                    const HttpStatusCode ok = HttpStatusCode.OK;

                    // We expect an OK Response to begin with.
                    Assert.Equal(ok, response.StatusCode);

                    var s = response.Content.ReadAsStringAsync().Result;

                    Assert.NotNull(s);
                    Assert.NotEqual(string.Empty, s);

                    // This is sufficient for test purposes.
                    var actualValues = JsonConvert.DeserializeObject<IEnumerable<int>>(s).ToArray();

                    Assert.Equal(MeasuredController.OddValues, actualValues);
                });
        }

        [Theory
        , InlineData(0, new int[0])
        , InlineData(1, new[] {1})
        , InlineData(2, new int[0])
        , InlineData(3, new[] {3})
        , InlineData(4, new int[0])
        , InlineData(5, new[] {5})
        , InlineData(6, new int[0])
        , InlineData(7, new[] {7})
        , InlineData(8, new int[0])
        , InlineData(9, new[] {9})
        ]
        public void VerifyApiVerifiedValueCorrect(int value, int[] expectedValues)
        {
            MakeRequest(client => client.GetAsync(string.Format("{0}/verify/{1}", BaseApiUrl, value)).Result)
                .Handle(response =>
                {
                    const HttpStatusCode ok = HttpStatusCode.OK;

                    Assert.NotNull(response);

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
