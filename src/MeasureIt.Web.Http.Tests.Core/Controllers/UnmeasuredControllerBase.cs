using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MeasureIt.Web.Http.Controllers
{
    public abstract class UnmeasuredControllerBase : ApiController
    {
        public IEnumerable<int> Get()
        {
            return EvenValues.ToArray();
        }

        [ActionName("verify")]
        public IEnumerable<int> Get(int value)
        {
            return EvenValues.Where(x => x == value).ToArray();
        }

        internal static IEnumerable<int> EvenValues
        {
            get
            {
                yield return 0;
                yield return 2;
                yield return 4;
                yield return 6;
                yield return 8;
            }
        }
    }
}
