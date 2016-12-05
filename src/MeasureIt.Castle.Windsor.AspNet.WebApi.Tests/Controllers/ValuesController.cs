using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MeasureIt.Castle.Windsor.AspNet.WebApi.Controllers
{
    public class ValuesController : ApiController
    {
        public IEnumerable<int> Get()
        {
            return ExpectedItems.ToArray();
        }

        [HttpGet, ActionName("verify")]
        public IEnumerable<int> Verify(int value)
        {
            return ExpectedItems.Where(x => x == value).ToArray();
        }

        internal static IEnumerable<int> ExpectedItems
        {
            get
            {
                yield return 1;
                yield return 2;
                yield return 3;
                yield return 4;
                yield return 5;
                yield return 6;
                yield return 7;
            }
        }
    }
}
