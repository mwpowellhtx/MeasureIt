using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MeasureIt.Castle.Windsor.AspNet.WebApi
{
    using Controllers;
    using Newtonsoft.Json;
    using Xunit;

    public class UnmeasuredControllerActionTests : SelfHostTestFixtureBase<StartupFixture>
    {
        private static string GetUrl()
        {
            // TODO: TBD: completely arbitrary port number(s)...
            var port = new Random().Next(9000, 10000 - 1);
            return string.Format("http://localhost:{0}/", port);
        }

        public UnmeasuredControllerActionTests()
            : base(GetUrl())
        {
        }

        [Fact]
        public void VerifyApiValuesCorrect()
        {
            // TODO: TBD: working out core Castle Windsor resolutions at the external library level...
            MakeRequest(client => client.GetAsync("api/values").Result)
                .Handle(response =>
                {
                    const HttpStatusCode expectedStatusCode = HttpStatusCode.OK;

                    Assert.NotNull(response);

                    Assert.Equal(expectedStatusCode, response.StatusCode);

                    var s = response.Content.ReadAsStringAsync().Result;

                    Assert.NotNull(s);
                    Assert.True(s.Length > 0);

                    var actualItems = JsonConvert.DeserializeObject<IEnumerable<int>>(s).ToArray();

                    Assert.True(actualItems.SequenceEqual(ValuesController.ExpectedItems));
                });
        }

        [Theory
        , InlineData(-1, new int[0])
        , InlineData(8, new int[0])
        , InlineData(1, new[] {1})
        , InlineData(2, new[] {2})
        , InlineData(3, new[] {3})
        , InlineData(4, new[] {4})
        ]
        public void VerifyApiVerifiedValueCorrect(int value, int[] expectedValues)
        {
            MakeRequest(client => client.GetAsync(string.Format("api/values/verify/{0}", value)).Result)
                .Handle(response =>
                {
                    const HttpStatusCode expectedStatusCode = HttpStatusCode.OK;

                    Assert.NotNull(response);

                    Assert.Equal(expectedStatusCode, response.StatusCode);

                    var s = response.Content.ReadAsStringAsync().Result;

                    Assert.NotNull(s);
                    Assert.True(s.Length > 0);

                    var actualItems = JsonConvert.DeserializeObject<IEnumerable<int>>(s).ToArray();

                    Assert.True(actualItems.SequenceEqual(expectedValues));
                });
        }
    }
}
